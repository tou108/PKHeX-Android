using PKHeX.Android.Pages;

namespace PKHeX.Android;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(SaveEditorPage), typeof(SaveEditorPage));
        Routing.RegisterRoute(nameof(BoxViewPage), typeof(BoxViewPage));
        Routing.RegisterRoute(nameof(PokemonEditorPage), typeof(PokemonEditorPage));
    }
}
