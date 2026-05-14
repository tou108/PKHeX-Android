using Android.App;
using Android.Content.PM;
using Android.OS;

namespace PKHeX.Android.Platforms.Android;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Handle file open intent (e.g., opened from file manager)
        if (Intent?.Action == global::Android.Content.Intent.ActionView && Intent.Data != null)
        {
            var uri = Intent.Data;
            // Pass the URI to the app via MessagingCenter or a service
            MessagingCenter.Send<MainActivity, global::Android.Net.Uri>(this, "OpenFileUri", uri);
        }
    }
}
