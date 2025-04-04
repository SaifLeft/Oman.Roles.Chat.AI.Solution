using MudBlazor;
using Button = MudBlazor.ButtonTypography;

namespace MauiStore.Infrastructure;
public class AppTheme
{
    private static Typography DefaultTypography = new Typography()
    {
        Default = new DefaultTypography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = "400",
            LineHeight = "1.43",
            LetterSpacing = ".01071em"
        },
        H1 = new H1Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "6rem",
            FontWeight = "300",
            LineHeight = "1.167",
            LetterSpacing = "-.01562em"
        },
        H2 = new H2Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "3.75rem",
            FontWeight = "300",
            LineHeight = "1.2",
            LetterSpacing = "-.00833em"
        },
        H3 = new H3Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "3rem",
            FontWeight = "400",
            LineHeight = "1.167",
            LetterSpacing = "0"
        },
        H4 = new H4Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "2.125rem",
            FontWeight = "400",
            LineHeight = "1.235",
            LetterSpacing = ".00735em"
        },
        H5 = new H5Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1.5rem",
            FontWeight = "400",
            LineHeight = "1.334",
            LetterSpacing = "0"
        },
        H6 = new H6Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1.25rem",
            FontWeight = "400",
            LineHeight = "1.6",
            LetterSpacing = ".0075em"
        },
        Button = new ButtonTypography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = "500",
            LineHeight = "1.75",
            LetterSpacing = ".02857em"
        },
        Body1 = new Body1Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1rem",
            FontWeight = "400",
            LineHeight = "1.5",
            LetterSpacing = ".00938em"
        },
        Body2 = new Body2Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = "400",
            LineHeight = "1.43",
            LetterSpacing = ".01071em"
        },
        Caption = new CaptionTypography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".75rem",
            FontWeight = "400",
            LineHeight = "1.2",
            LetterSpacing = ".03333em"
        },
        Subtitle1 = new Subtitle1Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = "1rem",
            FontWeight = "500",
            LineHeight = "1.5",
            LetterSpacing = ".00938em"
        },
        Subtitle2 = new Subtitle2Typography()
        {
            FontFamily = new[] { "Rubik", "Helvetica", "Arial", "sans-serif" },
            FontSize = ".875rem",
            FontWeight = "500",
            LineHeight = "1.2",
            LetterSpacing = ".00714em"
        }
    };

    private static LayoutProperties DefaultLayoutProperties = new LayoutProperties()
    {
        DefaultBorderRadius = "14px",
        DrawerWidthLeft = "260px",
        DrawerWidthRight = "300px"
    };

    public static MudTheme MyCustomTheme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#1040C1",
            Info = "#1072eb",
            AppbarBackground = "#FFF",
            AppbarText = "rgba(0,0,0, 0.7)",
            Background = "#F0F2F5",
            DrawerBackground = "#FFF",
            DrawerText = "rgba(0,0,0, 0.7)",
            DrawerIcon = "rgba(0,0,0, 0.6)",
            Success = "#018065",
            Error = "#dc3545",
            White = "#FFFFFF",
            Black = "#000000"
        },

        PaletteDark = new PaletteDark()
        {
            Primary = "#1040C1",
            Info = "#1072eb",
            Success = "#018065",
            Error = "#dc3545",
            White = "#FFFFFF",
            Black = "#27272f",
            Background = "#32333d",
            BackgroundGray = "#27272f",
            Surface = "#373740",
            DrawerBackground = "#27272f",
            DrawerText = "rgba(255,255,255, 0.50)",
            AppbarBackground = "#373740",
            AppbarText = "rgba(255,255,255, 0.70)",
            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            ActionDefault = "#adadb1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            DrawerIcon = "rgba(255,255,255, 0.50)"
        },
        Typography = DefaultTypography,
        LayoutProperties = DefaultLayoutProperties
    };
}
