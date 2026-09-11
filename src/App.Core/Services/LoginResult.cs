namespace App.Core.Services;

public enum LoginOutcome
{
    /// <summary>Connexion terminee : naviguer vers PAGE_Prince.</summary>
    Success,

    /// <summary>Plusieurs perimetres : afficher l'etape 2 (CELL_Connect plan 2).</summary>
    NeedsScopeSelection,

    /// <summary>Acces refuse : afficher le message tel quel.</summary>
    Denied
}

public record LoginResult(
    LoginOutcome Outcome,
    string? Message = null,
    List<string>? Scopes = null)
{
    public static LoginResult Success() =>
        new(LoginOutcome.Success);

    public static LoginResult NeedsScopeSelection(List<string> scopes) =>
        new(LoginOutcome.NeedsScopeSelection, null, scopes);

    public static LoginResult Denied(string message) =>
        new(LoginOutcome.Denied, message);
}
