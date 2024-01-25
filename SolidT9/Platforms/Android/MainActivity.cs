using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using NLog;

namespace SolidT9;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

    protected override void OnNewIntent(Intent intent)
    {
        _logger.Debug("New Intent: {0}", intent.Action);    

        base.OnNewIntent(intent);
    }
}
