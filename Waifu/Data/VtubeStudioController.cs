using System.DirectoryServices;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Humanizer;
using Microsoft.Extensions.Logging;
using VTS.Core;
using Waifu.Data.Args;

namespace Waifu.Data;

public class VtubeStudioController : VTS.Core.CoreVTSPlugin
{
    private readonly ILogger<VtubeStudioController> _logger;
    private readonly VtsLogger _vtsLogger;
    private readonly EventMaster _eventMaster;
    private readonly AudioLevelCalculator _audioLevelCalculator;

    private DispatcherTimer _audioLevelTimer = new()
    {
        Interval = TimeSpan.FromSeconds(0.2)
    };

    private AudioLevelData? _audioLevelData;

    public VtubeStudioController(ILogger<VtubeStudioController> logger, VtsLogger vtsLogger, EventMaster eventMaster,
        AudioLevelCalculator audioLevelCalculator) :
        base(vtsLogger, (int)TimeSpan.FromSeconds(0.2).TotalMilliseconds,
            Assembly.GetExecutingAssembly().GetName().Name.Humanize().Truncate(20, "..."),
            Environment.UserName.Truncate(20, "..."), null /* load icon??? */)
    {
        _logger = logger;
        _vtsLogger = vtsLogger;
        _eventMaster = eventMaster;
        _audioLevelCalculator = audioLevelCalculator;

        _audioLevelTimer.Tick += (sender, args) =>
        {
            if (_audioLevelData is not null)
            {
                _ = this.InjectParameterValues(new[]
                {
                    new VTSParameterInjectionValue()
                    {
                        id = "MouthOpen", value = (float)_audioLevelData.AudioLevel, weight = 1
                    }
                });
                logger.LogInformation($"Level in {_audioLevelData.AudioLevel}");
            }
        };

        _audioLevelTimer.Start();

        _audioLevelCalculator.AudioLevelCalculated += (sender, data) =>
        {
            if (data.AudioTarget == "CharacterVoice")
            {
                _audioLevelData = data;
            }
        };
    }

    public bool IsConnected { get; private set; } = false;

    public async Task<bool> ConnectToVtubeStudio()
    {
        try
        {
            if (File.Exists(Constants.DefaultLowIcon) && string.IsNullOrWhiteSpace(this.PluginIcon))
            {
                var pfpBytes = await File.ReadAllBytesAsync(Constants.DefaultLowIcon);

                this.PluginIcon = Convert.ToBase64String(pfpBytes);

                _logger.LogInformation($"Plugin icon set at {pfpBytes.Length.Bytes().Humanize()}");
            }


            await this.InitializeAsync(new WebSocketImpl(_vtsLogger), new NewtonsoftJsonUtilityImpl(),
                new TokenStorageImpl("./token"),
                () => { _eventMaster.TriggerInfo("VtubeStudio disconnected!"); });

            IsConnected = true;
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Attempt to connect to vtube studio error {exception.Message}");
        }

        return IsConnected;
    }

    public async Task AttemptToConnectForever()
    {
        var connectCancellationToken = new CancellationTokenSource();

        var connectTask = Task.Run(async () =>
        {
            var awaiter = new PeriodicTimer(TimeSpan.FromSeconds(3));

            while (!_eventMaster.IsShuttingDown && await awaiter.WaitForNextTickAsync(connectCancellationToken.Token))
            {
                if (!IsConnected)
                    await ConnectToVtubeStudio();
            }
        }, connectCancellationToken.Token);

        _eventMaster.ShuttingDown += (sender, args) =>
        {
            _audioLevelTimer.Stop();

            if (IsConnected)
                Disconnect();

            connectCancellationToken.Cancel();
        };
    }
}