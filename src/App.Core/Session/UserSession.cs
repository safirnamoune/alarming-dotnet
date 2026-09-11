namespace App.Core.Session;

/// <summary>
/// Remplace les variables globales de la collection ProceduresServeur du projet
/// WebDev (gsUserCode, gsUserName, gnScope_Id, gnSessionId, ...).
///
/// Enregistree en Scoped : en Blazor Server, une instance par connexion
/// utilisateur. C'est l'equivalent sur des globales WebDev, sans risque de
/// collision entre utilisateurs.
/// </summary>
public class UserSession
{
    // ProceduresServeur.gsUserCode
    public string UserCode { get; set; } = "";

    // ProceduresServeur.gsUserName
    public string UserName { get; set; } = "";

    // ProceduresServeur.gsUserlogin
    public string UserLogin { get; set; } = "";

    // ProceduresServeur.gsUserMail
    public string UserMail { get; set; } = "";

    // ProceduresServeur.gsUserManager
    public string UserManager { get; set; } = "";

    // ProceduresServeur.gsUserAgent - "SYSTEM" si aucun agent ne correspond
    public string UserAgent { get; set; } = "SYSTEM";

    // ProceduresServeur.gbUserAdmin
    public bool IsAdmin { get; set; }

    // ProceduresServeur.gnScope_Id / gsScope_Name
    public int ScopeId { get; set; }
    public string ScopeName { get; set; } = "";

    // ProceduresServeur.gsTeamId / gsTeamName
    public int TeamId { get; set; }
    public string TeamName { get; set; } = "";

    // ProceduresServeur.gnSessionId (audit_access.id_session)
    public int SessionId { get; set; }

    // ProceduresServeur.gnLangue_Utilisee : "fr" ou "en"
    public string Language { get; set; } = "fr";

    // ProceduresServeur.gsUser_public_key / gsUser_private_key
    // ANOMALIE A4 du registre : la cle privee est chargee en memoire de session,
    // comme dans le code WebDev. Reproduit tel quel en phase 1.
    public string? UserPublicKey { get; set; }
    public string? UserPrivateKey { get; set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(UserCode);

    public void Clear()
    {
        UserCode = UserName = UserLogin = UserMail = UserManager = "";
        UserAgent = "SYSTEM";
        IsAdmin = false;
        ScopeId = TeamId = SessionId = 0;
        ScopeName = TeamName = "";
        UserPublicKey = UserPrivateKey = null;
    }
}
