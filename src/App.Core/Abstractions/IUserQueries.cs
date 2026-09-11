using App.Core.Models;

namespace App.Core.Abstractions;

public interface IUserQueries
{
    // REQ_SysUsers filtre sur pUserLogin et pAppAccess
    Task<List<SysUser>> GetByLoginAsync(string login, CancellationToken ct = default);

    // REQ_Rch_Users_Scopes_View
    Task<List<ScopeDepartment>> GetScopeDepartmentsAsync(
        string userId, string scopeName, CancellationToken ct = default);

    // agents.LitRecherchePremier(userlogin, ...)
    Task<string?> GetAgentNameAsync(string userLogin, CancellationToken ct = default);

    // HLitRecherchePremier(scopes, scope_name, ...)
    Task<int?> GetScopeIdByNameAsync(string scopeName, CancellationToken ct = default);

    // HLitRecherchePremier(departement, DEPTNAME, ...)
    Task<int?> GetDeptIdByNameAsync(string deptName, CancellationToken ct = default);
}
