using System.DirectoryServices;
using System.Reflection;
using Humanizer;
using Microsoft.Extensions.Logging;
using VTS.Core;

namespace Waifu.Data;

public class VtubeStudioController : VTS.Core.CoreVTSPlugin
{
    private readonly ILogger<VtubeStudioController> _logger;
    private readonly VtsLogger _vtsLogger;
    private readonly EventMaster _eventMaster;

    public VtubeStudioController(ILogger<VtubeStudioController> logger, VtsLogger vtsLogger, EventMaster eventMaster) :
        base(vtsLogger, (int)TimeSpan.FromSeconds(0.2).TotalMilliseconds,
            Assembly.GetExecutingAssembly().GetName().Name.Humanize().Truncate(20, "..."),
            Environment.UserName.Truncate(20, "..."), string.Empty /* load icon??? */)
    {
        _logger = logger;
        _vtsLogger = vtsLogger;
        _eventMaster = eventMaster;
    }

    public bool IsConnected { get; private set; } = false;

    public async Task<bool> ConnectToVtubeStudio()
    {
        try
        {
       
            await this.InitializeAsync(new WebSocketImpl(_vtsLogger), new NewtonsoftJsonUtilityImpl(),
                new TokenStorageImpl("./token"),
                () => { _eventMaster.TriggerInfo("VtubeStudio disconnected!"); });

            IsConnected = true;
        }
        catch
        {
        }

        return IsConnected;
    }

    public async Task AttemptToConnectForever()
    {
        _ = Task.Run(async () =>
        {
            var awaiter = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (true && await awaiter.WaitForNextTickAsync())
            {
                if (!IsConnected)
                    await ConnectToVtubeStudio();
            }
        });
    }
}