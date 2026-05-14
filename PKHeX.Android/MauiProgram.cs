using Microsoft.Extensions.Logging;
using PKHeX.Android.Pages;
using PKHeX.Android.Services;
using PKHeX.Android.ViewModels;

namespace PKHeX.Android;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<SaveFileService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<SaveEditorViewModel>();
        builder.Services.AddTransient<PokemonEditorViewModel>();
        builder.Services.AddTransient<BoxViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<SaveEditorPage>();
        builder.Services.AddTransient<PokemonEditorPage>();
        builder.Services.AddTransient<BoxViewPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
