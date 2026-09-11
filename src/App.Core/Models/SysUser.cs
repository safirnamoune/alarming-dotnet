namespace App.Core.Models;

/// <summary>
/// Une ligne de la vue alarming_schema.users_view.
/// Equivalent du resultat de REQ_SysUsers.
/// </summary>
public record SysUser(
    string UserId,
    string UserName,
    string UserLogin,
    string UserMail,
    string UserManager,
    bool IsAdmin,
    bool ExternalUser,
    int ScopeId,
    string ScopeName,
    int DeptIdScope,
    string DeptNameScope);
