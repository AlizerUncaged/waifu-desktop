using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Autofac;
using AutofacSerilogIntegration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Waifu.Data;
using Waifu.Views;
using Waifu.Views.Index;

namespace Waifu;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    ContainerBuilder builder = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // register views
        builder.RegisterType<MainWindow>()
            .AsSelf();

        builder.RegisterType<Welcome>()
            .AsSelf();

        builder.RegisterType<HardwareSpecifications>()
            .AsSelf().As<ISelfRunning>();

        builder.RegisterType<ApplicationDbContext>()
            .InstancePerLifetimeScope();

        // set logs
        ConfigureLogger();
        builder.RegisterLogger();

        var container = builder.Build();

        var lifetime = container.BeginLifetimeScope();

        var mainWindow = lifetime.Resolve<MainWindow>();

        MainWindow = mainWindow;

        // run self running services.
        using (var scope = container.BeginLifetimeScope())
        {
            var selfRunningServices = scope.Resolve<IEnumerable<ISelfRunning>>();
            var startTasks = selfRunningServices.Select(service => service.StartAsync());

            _ = Task.WhenAll(startTasks);
        }

        mainWindow.Show();
    }

    public void ConfigureLogger()
    {
        string logFolderPath = "Logs";

        // Ensure the log folder exists
        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }

        string logFilePath = Path.Combine(logFolderPath, GetLogFileName());

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true)
            .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: null,
                retainedFileCountLimit: null,
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level}] ({SourceContext:l}) {NewLine}{Message}{Exception}{NewLine}")
            .CreateLogger();
    }

    string GetLogFileName()
    {
        // Use the current date to generate the dynamic filename
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        return $"{date}-waifu-logs.txt";
    }
}