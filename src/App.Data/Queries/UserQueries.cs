using App.Core.Abstractions;
using App.Core.Models;
using Dapper;
using Npgsql;

namespace App.Data.Queries;

public class UserQueries(NpgsqlDataSource db) : IUserQueries
{
    // Conversion de REQ_SysUsers.
    // Le WebDev passe 8 parametres optionnels ; seuls pUserLogin et pAppAccess
    // sont renseignes lors de la connexion (les autres sont a Null).
    private const string SqlByLogin = """
        SELECT userid          AS UserId,
               username       AS UserName,
               userlogin      AS UserLogin,
               usermail       AS UserMail,
               usermanager    AS UserManager,
               (isadmin = 1)        AS IsAdmin,
               (external_user = 1)  AS ExternalUser,
               scope_id       AS ScopeId,
               scope_name     AS ScopeName,
               deptid_scope   AS DeptIdScope,
               deptname_scope AS DeptNameScope
        FROM   alarming_schema.users_view
        WHERE  UPPER(userlogin) = @Login
          AND  appaccess = 1
        ORDER  BY scope_name
        """;

    public async Task<List<SysUser>> GetByLoginAsync(
        string login, CancellationToken ct = default)
    {
        await using var conn = await db.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<SysUser>(
            SqlByLogin, new { Login = login.ToUpperInvariant() });
        return rows.ToList();
    }

    // Conversion de REQ_Rch_Users_Scopes_View
    private const string SqlScopeDepts = """
        SELECT scope_id AS ScopeId,
               scope_name AS ScopeName,
               deptid   AS DeptId,
               deptname AS DeptName
        FROM   alarming_schema.users_scopes_view
        WHERE  userid = @UserId
          AND  scope_name = @ScopeName
        ORDER  BY deptname
        """;

    public async Task<List<ScopeDepartment>> GetScopeDepartmentsAsync(
        string userId, string scopeName, CancellationToken ct = default)
    {
        await using var conn = await db.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<ScopeDepartment>(
            SqlScopeDepts, new { UserId = userId, ScopeName = scopeName });
        return rows.ToList();
    }

    public async Task<string?> GetAgentNameAsync(
        string userLogin, CancellationToken ct = default)
    {
        const string sql = """
            SELECT cm_agent_name
            FROM   alarming_schema.agents
            WHERE  userlogin = @Login
            LIMIT  1
            """;
        await using var conn = await db.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<string?>(sql, new { Login = userLogin });
    }

    public async Task<int?> GetScopeIdByNameAsync(
        string scopeName, CancellationToken ct = default)
    {
        const string sql = """
            SELECT scope_id
            FROM   alarming_schema.scopes
            WHERE  scope_name = @Name
            LIMIT  1
            """;
        await using var conn = await db.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int?>(sql, new { Name = scopeName });
    }

    public async Task<int?> GetDeptIdByNameAsync(
        string deptName, CancellationToken ct = default)
    {
        const string sql = """
            SELECT deptid
            FROM   alarming_schema.departement
            WHERE  deptname = @Name
            LIMIT  1
            """;
        await using var conn = await db.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int?>(sql, new { Name = deptName });
    }
}
