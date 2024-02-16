using System.Windows;
using Microsoft.Extensions.Logging;
using SharedUtils;
using Waifu.Utilities;

namespace Waifu.Data;

public class ProperShutdownHandler
{
    private readonly CharacterAiApi _characterAiApi;
    private readonly ProcessUtilities _processUtilities;
    private readonly ILogger<ProperShutdownHandler> _logger;
    private readonly EventMaster _eventMaster;

    public ProperShutdownHandler(CharacterAiApi characterAiApi, ProcessUtilities processUtilities,
        ILogger<ProperShutdownHandler> logger, EventMaster eventMaster)
    {
        _characterAiApi = characterAiApi;
        _processUtilities = processUtilities;
        _logger = logger;
        _eventMaster = eventMaster;
    }

    public async Task ShutdownProperly()
    {
        _eventMaster.TriggerShutDown();

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

        Application.Current.Dispatcher.Invoke(() => { Environment.Exit(Environment.ExitCode); });
    }
}