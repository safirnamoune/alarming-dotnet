namespace App.Core.Models;

/// <summary>
/// Ligne inseree dans alarming_schema.audit_access.
/// Equivalent des parametres de REQ_ajout_audit_access.
/// </summary>
public record AccessAudit(
    string UserId,
    string Login,
    string AccessMode,
    short Etat,
    DateTime DateDebut,
    TimeSpan HeureDebut,
    string IpMachine,
    string NomMachine,
    string NomNavigateur);
