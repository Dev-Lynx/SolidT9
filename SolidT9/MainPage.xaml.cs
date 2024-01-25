using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using NLog;
using ILogger = NLog.ILogger;

namespace SolidT9;

public partial class MainPage : ContentPage
{
	int count = 0;
	private ILogger _logger;

    public MainPage()
	{
		InitializeComponent();
		_logger = LogManager.GetCurrentClassLogger();

    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    protected override void OnAppearing()
    {
        _logger.Debug("Appearing...");
        base.OnAppearing();
    }
}

