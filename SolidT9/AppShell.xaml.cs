using SolidT9.Services;
using static Android.Webkit.ConsoleMessage;

namespace SolidT9;

public partial class AppShell : Shell
{
    private readonly AppServiceManager _svmgr;

    public AppShell()
	{
        _svmgr = new AppServiceManager();

        InitializeComponent();

        
        Loaded += HandleShellLoaded;
	}

    private void HandleShellLoaded(object sender, EventArgs e)
    {
        _svmgr.InitServices();
    }
}
