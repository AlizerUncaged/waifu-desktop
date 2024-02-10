using System.IO;
using CharacterAI.Client;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Waifu.Data;

public class StartupCheck : ISelfRunning
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<StartupCheck> _logger;
    private readonly Hotkeys _hotkeys;

    public StartupCheck(ApplicationDbContext applicationDbContext, ILogger<StartupCheck> logger, Hotkeys hotkeys)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _hotkeys = hotkeys;
    }

    public async Task<bool> DoesAModelExistAsync()
    {
        Directory.CreateDirectory(Constants.ModelsFolder);

        return Directory.GetFiles(Constants.ModelsFolder, "*", SearchOption.AllDirectories).Any();
    }

    public async Task StartAsync()
    {
        Log("Updating Database");
        // make sure database is ok
        await _applicationDbContext.Database.MigrateAsync();

        Log("Checking Puppeteer");

        PuppeteerLib.PuppeteerLib.PuppeteerDownloadProcessChanged += (sender, i) =>
        {
            Log(
                $"Downloading Puppeteer {i.BytesReceived.Bytes().Humanize()}/{Convert.ToInt32(i.TotalBytesToReceive).Bytes().Humanize()}",
                true);
        };

        if (new CharacterAiClient() is { } chaiClient)
        {
            await chaiClient.DownloadBrowserAsync();
        }

        Log("Loading hotkeys");
        await _hotkeys.HookHotkeys();
        // cache hotkeys
        await _hotkeys.GetAllHotkeys();

        Log("Starting");
        OnCheckFinishedSuccessfully?.Invoke(this, EventArgs.Empty);
    }

    private void Log(string log, bool frontendOnly = false)
    {
        if (!frontendOnly)
            _logger.LogDebug(log);

        OnLogChanged?.Invoke(this, log);
    }


    public event EventHandler<string> OnLogChanged;
    public event EventHandler OnCheckFinishedSuccessfully;
}