using App.Core.Abstractions;

namespace App.Web.Services;

/// <summary>
/// Remplace NavigateurAdresseIP(), NetNomMachine() et NavigateurType()
/// du code WebDev, en conservant les MEMES libelles de navigateur afin que
/// les statistiques de la table audit_access restent comparables avant et
/// apres bascule.
/// </summary>
public class ClientInfoService(IHttpContextAccessor http) : IClientInfo
{
    public string Ip =>
        http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";

    /// <summary>
    /// NetNomMachine(sIP) : la resolution DNS inverse est couteuse et peu fiable.
    /// En phase 1 on renvoie la chaine vide, ce que le WebDev produisait deja
    /// dans la plupart des cas sur le reseau interne.
    /// </summary>
    public string MachineName => "";

    /// <summary>
    /// Reproduit le SELON sur NavigateurType() de PAGE_Splash.
    /// L'ordre des tests est important : "Edg" avant "Chrome", "Chrome" avant
    /// "Safari", sinon Edge serait journalise comme Chrome.
    /// </summary>
    public string BrowserName
    {
        get
        {
            var ua = http.HttpContext?.Request.Headers.UserAgent.ToString() ?? "";

            if (ua.Contains("Edg", StringComparison.OrdinalIgnoreCase))
                return "Edge";
            if (ua.Contains("OPR", StringComparison.OrdinalIgnoreCase)
                || ua.Contains("Opera", StringComparison.OrdinalIgnoreCase))
                return "Opera";
            if (ua.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                return "Chrome";
            if (ua.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
                return "Firefox";
            if (ua.Contains("Safari", StringComparison.OrdinalIgnoreCase))
                return "Safari";
            if (ua.Contains("bot", StringComparison.OrdinalIgnoreCase))
                return "Robot search engine";

            // Libelle exact du code WebDev (NavigateurTypeInconnu)
            return "The browser used is unknown or the information is missing";
        }
    }
}
