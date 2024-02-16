using System.IO;
using Microsoft.Extensions.Logging;
using Waifu.Models;

namespace Waifu.Data;

public class UpdateChecker
{
    private readonly ILogger<UpdateChecker> _logger;

    public UpdateChecker(ILogger<UpdateChecker> logger)
    {
        _logger = logger;
    }

    public event EventHandler<Update> UpdateFound;

    public async Task CheckUpdateAndNotifyFrontendAsync()
    {
        if (!File.Exists(Constants.UpdateJson))
        {
            _logger.LogError($"Update metadata file not found at {Constants.UpdateJson}");
            return;
        }
        
        
    }
}