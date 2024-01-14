using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Waifu.Data;

public class StartupCheck : ISelfRunning
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger _logger;

    public StartupCheck(ApplicationDbContext applicationDbContext, ILogger logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        Log("Migrating Database");
        await _applicationDbContext.Database.MigrateAsync();

        Log("Starting");

        OnCheckFinishedSuccessfully?.Invoke(this, EventArgs.Empty);
    }

    private void Log(string log)
    {
        _logger.Debug(log);
        OnLogChanged?.Invoke(this, log);
    }


    public event EventHandler<string> OnLogChanged;
    public event EventHandler OnCheckFinishedSuccessfully;
}