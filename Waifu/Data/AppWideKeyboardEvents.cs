using Microsoft.Extensions.Logging;

namespace Waifu.Data;

public class AppWideKeyboardEvents
{
    private readonly ILogger<AppWideKeyboardEvents> _logger;

    public AppWideKeyboardEvents(ILogger<AppWideKeyboardEvents> logger)
    {
        _logger = logger;
    }
}