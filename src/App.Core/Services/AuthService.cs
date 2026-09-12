using System.Security.Cryptography;
using App.Core.Abstractions;
using App.Core.Models;
using App.Core.Session;

namespace App.Core.Services;

/// <summary>
/// Conversion iso-fonctionnelle du traitement "Clic sur BTN_Submit_1st (serveur)"
/// et de la procedure locale Get_User_keys() de PAGE_Splash.
/// </summary>
public class AuthService(
    IUserQueries users,
    IAuditQueries audit,
    IUserKeyQueries userKeys,
    IClientInfo client,
    UserSession session)
{
    /// <summary>
    /// Valeur de audit_access.access_mode ecrite par le WebDev.
    ///
    /// Verifiee en base le 12/09/2026 : 689 lignes a 0, contre 2 a 1 qui sont
    /// les traces de nos propres essais du 11/09. La valeur 1 avait ete deduite
    /// a tort ; 0 est la valeur d'origine.
    ///
    /// A CONFIRMER : la signification exacte du champ reste inconnue. Aucune
    /// autre valeur n'apparait dans l'historique, donc le WebDev ne semble
    /// jamais en ecrire d'autre.
    /// </summary>
    private const short AccessModeWeb = 0;

    /// <summary>Equivalent de BTN_Submit_1st : premiere etape de connexion.</summary>
    public async Task<LoginResult> LoginAsync(
        string login, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return LoginResult.Denied(Messages.InvalidCredentials);

        // REQ_SysUsers : pUserLogin = Majuscule(SAI_UtilisateurCode), pAppAccess = Vrai
        var rows = await users.GetByLoginAsync(login, ct);

        // SI HEnDehors(REQ_SysUsers) ALORS Erreur(...)
        if (rows.Count == 0)
            return LoginResult.Denied(Messages.NotAuthorized);

        // ------------------------------------------------------------------
        // PHASE 1 - ISO-FONCTIONNEL : AUCUNE VERIFICATION DU MOT DE PASSE.
        //
        // Le code WebDev d'origine ne la fait pas :
        //   - la sequence LDAP est neutralisee par "SI 1= 1 ALORS //LDAPConnecte(...)"
        //   - REQ_SysUsers.pUserPassword est force a Null
        //
        // ANOMALIE A1 DU REGISTRE. NE PAS "CORRIGER" EN PHASE 1 : cela bloquerait
        // des utilisateurs qui se connectent aujourd'hui.
        // Correction prevue en phase 2 (Entra ID ou ASP.NET Core Identity).
        // ------------------------------------------------------------------
        _ = password;

        var u = rows[0];

        // Affectations des globales ProceduresServeur
        session.UserCode    = u.UserId;
        session.UserName    = u.UserName;
        session.UserLogin   = u.UserLogin;
        session.UserMail    = u.UserMail;
        session.UserManager = u.UserManager;
        session.IsAdmin     = u.IsAdmin;
        session.TeamId      = u.DeptIdScope;
        session.TeamName    = u.DeptNameScope;

        // REQ_ajout_audit_access puis recuperation de id_session
        session.SessionId = await audit.LogAccessAsync(new AccessAudit(
            UserId:        u.UserId,
            Login:         login,
            AccessMode:    AccessModeWeb,
            Etat:          1,                       // ProceduresServeur.gnConnecte
            DateDebut:     DateTime.Today,          // DateSys()
            HeureDebut:    DateTime.Now.TimeOfDay,  // HeureSys()
            IpMachine:     client.Ip,
            NomMachine:    client.MachineName,
            NomNavigateur: client.BrowserName), ct);

        // SI agents.LitRecherchePremier(userlogin, gsUserlogin) ALORS ... SINON "SYSTEM"
        session.UserAgent = await users.GetAgentNameAsync(u.UserLogin, ct) ?? "SYSTEM";

        // SI HNbEnr(REQ_SysUsers) > 1 ALORS ... CELL_Connect..Plan = 2
        var scopes = rows.Select(r => r.ScopeName)
                         .Where(s => !string.IsNullOrEmpty(s))
                         .Distinct()
                         .ToList();

        if (rows.Count > 1)
            return LoginResult.NeedsScopeSelection(scopes);

        // Cas d'un seul perimetre : acces direct.
        //
        // Verifie en base le 12/09/2026 : users_scopes_view est VIDE et
        // users_view renvoie scope_id / scope_name / deptid_scope /
        // deptname_scope a NULL pour tous les comptes. Ces affectations sont
        // donc nulles en pratique, et l'etape 2 ne se declenche jamais.
        session.ScopeId   = u.ScopeId;
        session.ScopeName = u.ScopeName;
        session.TeamId    = u.DeptIdScope;
        session.TeamName  = u.DeptNameScope;

        await EnsureUserKeysAsync(ct);
        return LoginResult.Success();
    }

    /// <summary>Equivalent de init_scopes_departements() : alimente COMBO_Group.</summary>
    public async Task<List<string>> GetDepartmentsAsync(
        string scopeName, CancellationToken ct = default)
    {
        var rows = await users.GetScopeDepartmentsAsync(session.UserCode, scopeName, ct);
        return rows.Select(r => r.DeptName)
                   .Where(d => !string.IsNullOrEmpty(d))
                   .Distinct()
                   .ToList();
    }

    /// <summary>Equivalent de "Clic sur BTN_Submit_2nd (serveur)".</summary>
    public async Task<LoginResult> ConfirmScopeAsync(
        string scopeName, string deptName, CancellationToken ct = default)
    {
        var scopeId = await users.GetScopeIdByNameAsync(scopeName, ct);
        var deptId  = await users.GetDeptIdByNameAsync(deptName, ct);

        session.ScopeId   = scopeId ?? 0;
        session.ScopeName = scopeName;
        session.TeamId    = deptId ?? 0;
        session.TeamName  = deptName;

        await EnsureUserKeysAsync(ct);
        return LoginResult.Success();
    }

    /// <summary>
    /// Equivalent de la procedure locale Get_User_keys() de PAGE_Splash.
    /// Genere une paire RSA a la premiere connexion et la stocke en base.
    ///
    /// ANOMALIE A4 DU REGISTRE : stocker la cle privee en base est une mauvaise
    /// pratique. Reproduit tel quel en phase 1, a revoir en phase 2.
    /// </summary>
    private async Task EnsureUserKeysAsync(CancellationToken ct)
    {
        var existing = await userKeys.GetAsync(session.UserCode, ct);

        if (existing is not null)
        {
            session.UserPublicKey  = existing.PublicKey;
            session.UserPrivateKey = existing.PrivateKey;
            return;
        }

        // CrypteGenereCleRSA()
        using var rsa = RSA.Create(2048);
        var keys = new UserKeys(
            PublicKey:  Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
            PrivateKey: Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()));

        await userKeys.InsertAsync(session.UserCode, keys, ct);

        session.UserPublicKey  = keys.PublicKey;
        session.UserPrivateKey = keys.PrivateKey;
    }
}

