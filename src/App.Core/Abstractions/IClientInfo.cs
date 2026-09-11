namespace App.Core.Abstractions;

/// <summary>
/// Remplace NavigateurAdresseIP(), NetNomMachine() et NavigateurType()
/// du code WebDev.
/// </summary>
public interface IClientInfo
{
    string Ip { get; }
    string MachineName { get; }
    string BrowserName { get; }
}
