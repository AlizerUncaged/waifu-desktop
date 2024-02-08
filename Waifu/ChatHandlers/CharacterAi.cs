using CharacterAI.Client;
using Waifu.Models;
using Settings = Waifu.Data.Settings;

namespace Waifu.ChatHandlers;

public class CharacterAi : IChatHandler
{
    private readonly Settings _settings;
    public event EventHandler<string>? CompleteMessageGenerated;

    public event EventHandler<string>? PartialMessageGenerated;

    public ChatChannel ChatChannel { get; set; }

    public CharacterAiClient? CharacterAiClient { get; set; }

    public CharacterAi(Settings settings)
    {
        _settings = settings;

        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            if (CharacterAiClient is not null)
                CharacterAiClient.EnsureAllChromeInstancesAreKilled();
        };
    }

    private bool isInitialized = false;
    private SemaphoreSlim initSemaphore = new(1);

    public async Task InitializeAsync()
    {
        await initSemaphore.WaitAsync();

        if (isInitialized)
            return;

        var currentSettings = await _settings.GetOrCreateSettings();

        var chaiToken = currentSettings.CharacterAiToken;

        CharacterAiClient = new CharacterAiClient(chaiToken);

        await CharacterAiClient.LaunchBrowserAsync();

        isInitialized = true;

        initSemaphore.Release();
    }

    public async Task<string?> SendMessageAndGetResultAsync(string message)
    {
        if (!isInitialized)
            await InitializeAsync();

        return null;
    }
}