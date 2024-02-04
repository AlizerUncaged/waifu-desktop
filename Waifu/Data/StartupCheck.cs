using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Waifu.Data;

public class StartupCheck : ISelfRunning
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<StartupCheck> _logger;

    public StartupCheck(ApplicationDbContext applicationDbContext, ILogger<StartupCheck> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
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

        Log("Starting");
        OnCheckFinishedSuccessfully?.Invoke(this, EventArgs.Empty);
    }

    private void Log(string log)
    {
        _logger.LogDebug(log);
        OnLogChanged?.Invoke(this, log);
    }


    public event EventHandler<string> OnLogChanged;
    public event EventHandler OnCheckFinishedSuccessfully;
}