using PKHeX.Android.ViewModels;

namespace PKHeX.Android.Pages;

public partial class BoxViewPage : ContentPage
{
    private readonly BoxViewModel _vm;

    public BoxViewPage(BoxViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Initialize();
    }
}
