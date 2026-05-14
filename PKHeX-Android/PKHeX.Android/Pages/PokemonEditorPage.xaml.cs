using PKHeX.Android.ViewModels;

namespace PKHeX.Android.Pages;

public partial class PokemonEditorPage : ContentPage
{
    private readonly PokemonEditorViewModel _vm;

    public PokemonEditorPage(PokemonEditorViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadPokemon();
    }
}
