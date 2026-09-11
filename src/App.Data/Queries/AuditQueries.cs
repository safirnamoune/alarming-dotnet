using App.Core.Abstractions;
using App.Core.Models;
using Dapper;
using Npgsql;

namespace App.Data.Queries;

public class AuditQueries(NpgsqlDataSource db) : IAuditQueries
{
    // Conversion de REQ_ajout_audit_access.
    // RETURNING remplace le "SELECT max(Id_Session) FROM audit_access" du WebDev
    // (anomalie A5 : exception autorisee, resultat identique sans collision).
    private const string Sql = """
        INSERT INTO alarming_schema.audit_access
               (userid, login, access_mode, etat,
                date_debut, heure_debut, date_fin, heure_fin,
                ip_machine, nom_machine, nom_navigateur, nbre_tentatives)
        VALUES (@UserId, @Login, @AccessMode, @Etat,
                @DateDebut, @HeureDebut, @DateDebut, @HeureDebut,
                @IpMachine, @NomMachine, @NomNavigateur, 1)
        RETURNING id_session
        """;

    public async Task<int> LogAccessAsync(
        AccessAudit audit, CancellationToken ct = default)
    {
        await using var conn = await db.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(Sql, audit);
    }
}
