using HardwareInformation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Waifu.Data;

public class HardwareSpecifications : ISelfRunning
{
    private readonly ILogger<HardwareSpecifications> _logger;

    public HardwareSpecifications(ILogger<HardwareSpecifications> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync()
    {
        try
        {
            var hardwareInformation =
                await Task.Run(() => MachineInformationGatherer.GatherInformation(skipClockspeedTest: true));

            var jsonInformation = JsonConvert.SerializeObject(hardwareInformation, Formatting.Indented);

            // send this for github issues
            _logger.LogDebug($"Hardware: {Environment.NewLine}{jsonInformation}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed getting hardware specifications: {Environment.NewLine}{ex}");
        }
    }
}