using App.Core.Models;

namespace App.Core.Abstractions;

public interface IAuditQueries
{
    /// <summary>
    /// Insere la trace de connexion et retourne id_session.
    ///
    /// EXCEPTION AUTORISEE (anomalie A5 du registre) : le code WebDev faisait
    /// INSERT puis SELECT max(Id_Session), ce qui peut renvoyer l'identifiant
    /// d'une autre connexion simultanee. On utilise RETURNING : resultat
    /// identique, sans condition de concurrence. Aucun impact visible.
    /// </summary>
    Task<int> LogAccessAsync(AccessAudit audit, CancellationToken ct = default);
}
