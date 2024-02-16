using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;
using Autofac;
using LLama.Common;
using MdXaml;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Autofac.DependencyInjection;
using Serilog.Sinks.SystemConsole.Themes;
using Waifu.ChatHandlers;
using Waifu.Controllers;
using Waifu.Data;
using Waifu.Data.HuggingFace;
using Waifu.Utilities;
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

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        };

        var logPath = Path.Combine(Constants.DataFolder,
            "log",
            $"extUI-{DateTime.Now:yyyy-MM-dd}-Log.log");

        var logConfiguration = new LoggerConfiguration()
            .WriteTo.Console(
                applyThemeToRedirectedOutput: true,
                theme: AnsiConsoleTheme.Code, restrictedToMinimumLevel: LogEventLevel.Debug,
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext}.{Method})  {Message:lj} {NewLine}{Exception}"
            ).WriteTo.File(logPath)
            .MinimumLevel.Information();

        //
        // Log.Logger = logConfiguration
        //     .CreateLogger();

        builder.RegisterSerilog(logConfiguration);

        // register views
        builder.RegisterType<MainWindow>()
            .AsSelf().SingleInstance();

        builder.RegisterType<Welcome>()
            .AsSelf();

        builder.RegisterType<Header>()
            .AsSelf();

        builder.RegisterType<HttpClient>()
            .AsSelf();

        builder.RegisterType<InformationArea>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<UpdateChecker>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<EventMaster>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<VtubeStudioController>()
            .AsSelf().SingleInstance();

        builder.RegisterType<VtsLogger>()
            .AsSelf();

        builder.RegisterType<Messages>()
            .AsSelf().InstancePerLifetimeScope();

        builder.RegisterType<MainArea>()
            .AsSelf();

        builder.RegisterType<WatcherManager>()
            .AsSelf();

        builder.RegisterType<ImageHashing>()
            .AsSelf();

        builder.RegisterType<WhisperManager>()
            .AsSelf().SingleInstance();

        builder.RegisterType<AudioRecorder>()
            .AsSelf().SingleInstance();

        builder.RegisterType<AudioLevelCalculator>()
            .AsSelf().SingleInstance();

        builder.RegisterType<HotkeyManager>()
            .AsSelf().SingleInstance();

        builder.RegisterType<Hotkeys>()
            .AsSelf().SingleInstance();

        builder.RegisterType<AtarashiCharacter>()
            .AsSelf().InstancePerDependency();

        builder.RegisterType<CharactersMenu>()
            .AsSelf().SingleInstance();

        builder.RegisterType<Waifu.Data.Settings>()
            .AsSelf().SingleInstance();

        // builder.RegisterType<CharacterAiChatHandler>()
        //     .As<IChatHandler>()
        //     .AsSelf();

        // builder.RegisterType<LocalLlama>()
        //     .As<IChatHandler>()
        //     .AsSelf();

        builder.RegisterType<Views.Index.Settings>()
            .AsSelf().InstancePerLifetimeScope();

        builder.RegisterType<Personas>()
            .AsSelf().InstancePerLifetimeScope();

        builder.RegisterType<ProcessUtilities>()
            .AsSelf();


        builder.RegisterType<ApplicationDbContextFactory>()
            .AsSelf().SingleInstance();

        builder.RegisterType<ChatServiceManager>()
            .AsSelf();
        builder.RegisterType<AudioPlayer>()
            .AsSelf();

        builder.RegisterType<ProperShutdownHandler>()
            .AsSelf();

        builder.RegisterType<ModelManager>()
            .AsSelf();

        builder.RegisterType<CharacterAiApi>()
            .AsSelf().SingleInstance();

        builder.RegisterType<AtarashiCharacterAi>()
            .AsSelf().InstancePerDependency();

        builder.RegisterType<HuggingFaceModelApi>()
            .AsSelf();

        builder.RegisterType<WhisperHuggingFaceModelDownloader>()
            .AsSelf().SingleInstance();


        builder.RegisterType<ChatAreaController>()
            .AsSelf().SingleInstance();

        builder.RegisterType<ElevenlabsVoiceGenerator>()
            .AsSelf().As<IVoiceGenerator>().SingleInstance();

        builder.RegisterType<ChatArea>()
            .InstancePerLifetimeScope()
            .AsSelf();

        builder.RegisterType<CharacterEdit>()
            .InstancePerDependency()
            .AsSelf();

        builder.RegisterType<Characters>()
            .AsSelf().SingleInstance();

        builder.RegisterType<HardwareSpecifications>()
            .AsSelf().As<ISelfRunning>();

        builder.RegisterType<StartupCheck>()
            .AsSelf().As<ISelfRunning>()
            .SingleInstance();

        builder.RegisterType<ApplicationDbContext>()
            .InstancePerDependency();


        var container = builder.Build();

        var lifetime = container.BeginLifetimeScope();

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


        var mainWindow = lifetime.Resolve<MainWindow>();

        MainWindow = mainWindow;

        mainWindow.Show();
    }
}