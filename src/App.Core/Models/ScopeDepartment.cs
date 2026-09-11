namespace App.Core.Models;

/// <summary>Resultat de REQ_Rch_Users_Scopes_View.</summary>
public record ScopeDepartment(
    int ScopeId,
    string ScopeName,
    int DeptId,
    string DeptName);
