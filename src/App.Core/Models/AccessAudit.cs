namespace App.Core.Models;

/// <summary>
/// Ligne inseree dans alarming_schema.audit_access.
/// Equivalent des parametres de REQ_ajout_audit_access.
///
/// access_mode est un smallint en base, pas un libelle texte.
/// </summary>
public record AccessAudit(
    string UserId,
    string Login,
    short AccessMode,
    short Etat,
    DateTime DateDebut,
    TimeSpan HeureDebut,
    string IpMachine,
    string NomMachine,
    string NomNavigateur);
