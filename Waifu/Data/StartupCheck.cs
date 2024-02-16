using System.IO;
using System.Threading.Channels;
using CharacterAI.Client;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Whisper.net.Ggml;
using ILogger = Serilog.ILogger;

namespace Waifu.Data;

public class StartupCheck : ISelfRunning
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<StartupCheck> _logger;
    private readonly Hotkeys _hotkeys;
    private readonly WhisperHuggingFaceModelDownloader _whisperHuggingFaceModelDownloader;
    private readonly CharacterAiApi _characterAiApi;
    private readonly Settings _settings;
    private readonly VtubeStudioController _vtubeStudioController;
    private readonly Messages _messages;

    public StartupCheck(ApplicationDbContext applicationDbContext, ILogger<StartupCheck> logger, Hotkeys hotkeys,
        WhisperHuggingFaceModelDownloader whisperHuggingFaceModelDownloader, CharacterAiApi characterAiApi,
        Settings settings, VtubeStudioController vtubeStudioController, Messages messages)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _hotkeys = hotkeys;
        _whisperHuggingFaceModelDownloader = whisperHuggingFaceModelDownloader;
        _characterAiApi = characterAiApi;
        _settings = settings;
        _vtubeStudioController = vtubeStudioController;
        _messages = messages;
    }

    public async Task<bool> DoesAModelExistAsync()
    {
        Directory.CreateDirectory(Constants.ModelsFolder);

        return Directory.GetFiles(Constants.ModelsFolder, "*", SearchOption.AllDirectories).Any();
    }

    public async Task StartAsync()
    {
        Log("Updating Database");
        await _applicationDbContext.Database.MigrateAsync();
        var settings = await _settings.GetOrCreateSettings();
        
        await _messages.GetAllMessagesAsync();

        Log("Checking Puppeteer");

        PuppeteerLib.PuppeteerLib.PuppeteerDownloadProcessChangedOptimized += (sender, i) =>
        {
            Log(
                $"Downloading Puppeteer {i.BytesReceived.Bytes().Humanize()}/{Convert.ToInt32(i.TotalBytesToReceive).Bytes().Humanize()}",
                true);
        };


        await _characterAiApi.InitializeAsync();


        Log("Loading hotkeys");

        _ = _hotkeys.HookHotkeys();

        // cache hotkeys
        await _hotkeys.GetAllHotkeys();

        Log("Downloading Whisper Base");

        var whisperAwaiter = new SemaphoreSlim(0, 1);

        var whisperModel = settings.WhisperModel;

        var progressEvent = _whisperHuggingFaceModelDownloader
            .DownloadWhisperModelInBackgroundAndSetAsModel(whisperModel, true);

        if (progressEvent is not null)
        {
            progressEvent.OptimizedProgressChanged += (sender, l) =>
            {
                Log($"Downloading Whisper {whisperModel} {l.Bytes().Humanize()}", true);
            };

            progressEvent.DownloadDone += (sender, args) => { whisperAwaiter.Release(); };

            await whisperAwaiter.WaitAsync();
        }

        // Log("Connecting to VTubeStudio");
        // if (!await _vtubeStudioController.ConnectToVtubeStudio())
        // {
        //     // did not connect
        // }

        Log("Starting");
        OnCheckFinishedSuccessfully?.Invoke(this, EventArgs.Empty);
    }

    public string CurrentLog { get; private set; }

    private void Log(string log, bool frontendOnly = false)
    {
        CurrentLog = log;
        if (!frontendOnly)
            _logger.LogDebug(log);

        OnLogChanged?.Invoke(this, log);
    }


    public event EventHandler<string> OnLogChanged;
    public event EventHandler OnCheckFinishedSuccessfully;
}