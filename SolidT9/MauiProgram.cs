using DevLynx.Log.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using LogLevel = NLog.LogLevel;
using ILogger = NLog.ILogger;
using SolidT9.Extensions;
using Microsoft.Maui.Embedding;
using System.Runtime.CompilerServices;

namespace SolidT9;

public static class MauiProgram
{
	private static ILogger _logger;

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiEmbedding<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		ConfigureLogging();
        builder.Logging.AddNLog();
#if DEBUG
		builder.Logging.AddDebug();
#endif
		
		_logger = LogManager.GetCurrentClassLogger();
		_logger.Debug($"SolidT9 Started");

        ErrorWatch.UnhandledException += HandleErrors;

		MauiContext ctx = null;
		builder.Services.AddSingleton(() => ctx);

        MauiApp app = builder.Build();
        ctx = new MauiContext(app.Services);

		ServiceProvider.Initialize(app.Services);

        return app;
	}

    private static void HandleErrors(object sender, UnhandledExceptionEventArgs e)
    {
		_logger.Error(e.ExceptionObject);
    }

    private static void ConfigureLogging()
	{
        var config = new LoggingConfiguration();

        // TODO: Add file logging in the future
#if DEBUG
        config.AddRemoteTarget("192.168.0.122", LogLevel.Trace);
#endif


        LogManager.Configuration = config;
        LogManager.ReconfigExistingLoggers();
    }
}
