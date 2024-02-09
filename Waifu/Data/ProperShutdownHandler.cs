using Microsoft.Extensions.Logging;
using SharedUtils;
using Waifu.Utilities;

namespace Waifu.Data;

public class ProperShutdownHandler
{
    private readonly CharacterAiApi _characterAiApi;
    private readonly ProcessUtilities _processUtilities;
    private readonly ILogger<ProperShutdownHandler> _logger;

    public ProperShutdownHandler(CharacterAiApi characterAiApi, ProcessUtilities processUtilities,
        ILogger<ProperShutdownHandler> logger)
    {
        _characterAiApi = characterAiApi;
        _processUtilities = processUtilities;
        _logger = logger;
    }

    public async Task ShutdownProperly()
    {
        _characterAiApi.CharacterAiClient?.EnsureAllChromeInstancesAreKilled();

        if (Common.CommonDirectory is { } browserDirectory &&
            !string.IsNullOrWhiteSpace(browserDirectory))
        {
            var processesRunningInBrowserDirectory =
                await _processUtilities.GetProcessesRunningInFolderAsync(browserDirectory);

            _logger.LogInformation(
                $"Killing {processesRunningInBrowserDirectory.Count()} processes at browserDirectory");

            foreach (var process in processesRunningInBrowserDirectory)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Unable to kill {process.ProcessName} because {ex.Message}");
                }
            }
        }
    }
}