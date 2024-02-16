using Microsoft.Extensions.Logging;
using VTS.Core;

namespace Waifu.Data;

public class VtsLogger : IVTSLogger
{
    private readonly ILogger<VtsLogger> _logger;

    public VtsLogger(ILogger<VtsLogger> logger)
    {
        _logger = logger;
    }

    public void Log(string message)
    {
        _logger.LogInformation(message);
    }

    public void LogWarning(string warning)
    {
        _logger.LogWarning(warning);
    }

    public void LogError(string error)
    {
        _logger.LogError(error);
    }

    public void LogError(Exception error)
    {
        _logger.LogError(error.ToString());
    }
}