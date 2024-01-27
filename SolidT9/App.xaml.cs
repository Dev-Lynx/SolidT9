using SolidT9.Services;

namespace SolidT9;

public partial class App : Application
{
    private readonly AppServiceManager _svmgr;

    public App()
	{
		InitializeComponent();

        _svmgr = new AppServiceManager();
        _svmgr.InitServices();

        MainPage = new AppShell();
	}
}
