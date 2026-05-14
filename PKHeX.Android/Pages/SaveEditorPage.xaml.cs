using PKHeX.Android.ViewModels;

namespace PKHeX.Android.Pages;

public partial class SaveEditorPage : ContentPage
{
    private readonly SaveEditorViewModel _vm;

    public SaveEditorPage(SaveEditorViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadFromSave();
    }

    private async void OnViewBoxClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BoxViewPage));
    }
}
