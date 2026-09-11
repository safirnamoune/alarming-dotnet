using MudBlazor;

namespace App.Web.Theme;

/// <summary>
/// Charte graphique Alarming. Couleur de marque : RVB 192/0/0 = #C00000.
/// Un seul endroit a modifier pour changer l'identite visuelle de toute
/// l'application.
/// </summary>
public static class AlarmingTheme
{
    public const string Brand      = "#C00000";
    public const string BrandDark  = "#8F0000";
    public const string BrandLight = "#E23B3B";

    public static readonly MudTheme Instance = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary              = Brand,
            PrimaryContrastText  = "#FFFFFF",
            Secondary            = "#1B1D21",
            Tertiary             = BrandLight,
            Background           = "#F4F5F7",
            BackgroundGray       = "#EDEEF1",
            Surface              = "#FFFFFF",
            AppbarBackground     = "#1B1D21",
            AppbarText           = "#FFFFFF",
            DrawerBackground     = "#FFFFFF",
            DrawerText           = "#14161A",
            TextPrimary          = "#14161A",
            TextSecondary        = "#6B7280",
            ActionDefault        = "#6B7280",
            Divider              = "#E5E7EB",
            LinesDefault         = "#E5E7EB",
            TableLines           = "#EDEEF1",
            Error                = "#C0392B",
            Success              = "#0E9F6E",
            Warning              = "#D97706",
            Info                 = "#2563EB",
        },

        PaletteDark = new PaletteDark
        {
            Primary          = BrandLight,
            Secondary        = "#F4F5F7",
            Background       = "#16181C",
            Surface          = "#1E2125",
            AppbarBackground = "#101215",
            TextPrimary      = "#F4F5F7",
            TextSecondary    = "#9CA3AF",
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
        },
    };
}
