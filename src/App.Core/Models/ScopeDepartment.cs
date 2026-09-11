namespace App.Core.Models;

/// <summary>Resultat de REQ_Rch_Users_Scopes_View.</summary>
public class ScopeDepartment
{
    public short ScopeId { get; set; }
    public string ScopeName { get; set; } = "";
    public int DeptId { get; set; }
    public string DeptName { get; set; } = "";
}
