using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Autofac;
using AutofacSerilogIntegration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Waifu.Data;
using Waifu.Data.HuggingFace;
using Waifu.Views;
using Waifu.Views.Index;
using Waifu.Views.Shared;
using Waifu.Views.Shared.Popups;

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
        builder.RegisterType<Header>()
            .AsSelf();

        builder.RegisterType<MainArea>()
            .AsSelf();

        builder.RegisterType<ImageHashing>()
            .AsSelf();

        builder.RegisterType<AtarashiCharacter>()
            .AsSelf();

        builder.RegisterType<CharactersMenu>()
            .AsSelf();

        builder.RegisterType<Settings>()
            .AsSelf();

        builder.RegisterType<ModelManager>()
            .AsSelf();

        builder.RegisterType<HuggingFaceModelApi>()
            .AsSelf();

        builder.RegisterType<Llama>()
            .AsSelf();

        builder.RegisterType<Characters>()
            .AsSelf().SingleInstance();

        builder.RegisterType<HardwareSpecifications>()
            .AsSelf().As<ISelfRunning>();

        builder.RegisterType<StartupCheck>()
            .AsSelf().As<ISelfRunning>()
            .SingleInstance();

        builder.RegisterType<ApplicationDbContext>()
            .InstancePerLifetimeScope();

        // set logs
        ConfigureLogger();
        builder.RegisterLogger();

        var container = builder.Build();

        var lifetime = container.BeginLifetimeScope();

        var mainWindow = lifetime.Resolve<MainWindow>();

        MainWindow = mainWindow;

        mainWindow.Show();

        // run self running services.
        _ = Task.Run(async () =>
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var selfRunningServices = scope.Resolve<IEnumerable<ISelfRunning>>();

                var startTasks = selfRunningServices.Select(service => service.StartAsync());

                await Task.WhenAll(startTasks);
            }
        });
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
            .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true,
                restrictedToMinimumLevel: LogEventLevel.Information)
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