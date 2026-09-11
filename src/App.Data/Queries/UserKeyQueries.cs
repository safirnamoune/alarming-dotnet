using App.Core.Abstractions;
using App.Core.Models;
using Dapper;
using Npgsql;

namespace App.Data.Queries;

public class UserKeyQueries(NpgsqlDataSource db) : IUserKeyQueries
{
    // Conversion de REQ_Rch_User_Keys
    public async Task<UserKeys?> GetAsync(
        string userId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT user_public_key  AS PublicKey,
                   user_private_key AS PrivateKey
            FROM   alarming_schema.user_keys
            WHERE  userid = @UserId
            LIMIT  1
            """;
        await using var conn = await db.OpenConnectionAsync(ct);
        return await conn.QueryFirstOrDefaultAsync<UserKeys>(sql, new { UserId = userId });
    }

    // Conversion de REQ_Ajout_User_Keys
    public async Task InsertAsync(
        string userId, UserKeys keys, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO alarming_schema.user_keys
                   (userid, user_public_key, user_private_key)
            VALUES (@UserId, @PublicKey, @PrivateKey)
            """;
        await using var conn = await db.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(sql, new
        {
            UserId = userId,
            keys.PublicKey,
            keys.PrivateKey
        });
    }
}
