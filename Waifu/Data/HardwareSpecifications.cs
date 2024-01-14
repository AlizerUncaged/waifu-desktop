using HardwareInformation;
using Newtonsoft.Json;
using Serilog;

namespace Waifu.Data;

public class HardwareSpecifications : ISelfRunning
{
    private readonly ILogger _logger;

    public HardwareSpecifications(ILogger logger)
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
            _logger.Debug($"Hardware: {Environment.NewLine}{jsonInformation}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed getting hardware specifications: {Environment.NewLine}{ex}");
        }
    }
}