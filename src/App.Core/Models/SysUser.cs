namespace App.Core.Models;

/// <summary>
/// Une ligne de la vue alarming_schema.users_view.
/// Equivalent du resultat de REQ_SysUsers.
///
/// Classe a proprietes modifiables et NON record positionnel : Dapper a besoin
/// d'un constructeur sans parametre pour tolerer les ecarts de type entre
/// PostgreSQL et .NET (smallint -> short, colonnes nulles, ordre des colonnes).
/// </summary>
public class SysUser
{
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string UserLogin { get; set; } = "";
    public string UserMail { get; set; } = "";
    public string UserManager { get; set; } = "";
    public bool IsAdmin { get; set; }
    public bool ExternalUser { get; set; }
    public short ScopeId { get; set; }
    public string ScopeName { get; set; } = "";
    public int DeptIdScope { get; set; }
    public string DeptNameScope { get; set; } = "";
}
