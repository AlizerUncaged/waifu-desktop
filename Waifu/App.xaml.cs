using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Autofac;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Autofac.DependencyInjection;
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

        var logPath = Path.Combine(Constants.DataFolder,
            "log",
            $"extUI-{DateTime.Now:yyyy-MM-dd}-Log.log");

        var logConfiguration = new LoggerConfiguration()
            .WriteTo.Console(
                applyThemeToRedirectedOutput: true,
                theme: AnsiConsoleTheme.Code, restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}"
            ).WriteTo.File(logPath)
            .MinimumLevel.Information();
        
        //
        // Log.Logger = logConfiguration
        //     .CreateLogger();

        builder.RegisterSerilog(logConfiguration);

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
}