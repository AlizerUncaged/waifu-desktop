using CharacterAI.Client;
using CharacterAI.Models;
using Microsoft.Extensions.Logging;
using Waifu.Views;

namespace Waifu.Data;

public class CharacterAiApi
{
    private readonly Settings _settings;
    private readonly ILogger<CharacterAiApi> _logger;

    public CharacterAiClient? CharacterAiClient { get; set; }

    public CharacterAiApi(Settings settings, ILogger<CharacterAiApi> logger)
    {
        _settings = settings;
        _logger = logger;

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

        _logger.LogDebug("Initializing chrome browser");

        var currentSettings = await _settings.GetOrCreateSettings();

        var chaiToken = currentSettings.CharacterAiToken;

        CharacterAiClient = new CharacterAiClient(chaiToken);

        await CharacterAiClient.LaunchBrowserAsync();

        isInitialized = true;
        
        _logger.LogInformation("Chrome browser initialized!");

        initSemaphore.Release();
    }

    public event EventHandler<string> ApiNotificationMessage;

    /// <summary>
    /// Gets a possible error. Null if no errors.
    /// </summary>
    public async Task<bool> CheckCharacterAiToken()
    {
        var settings = await _settings.GetOrCreateSettings();

        if (string.IsNullOrWhiteSpace(settings.CharacterAiToken))
        {
            Log("No character.ai token set!");
            return false;
        }

        return true;
    }

    public async Task<string> GenerateNewChannelAndGetChatIdAsync(string characterId)
    {
        if (!isInitialized)
            await InitializeAsync();

        return await CharacterAiClient.CreateNewChatAsync(characterId);
    }


    public async Task<Character> GetCharacterDataFromId(string characterId)
    {
        if (!isInitialized)
            await InitializeAsync();

        var character = await CharacterAiClient.GetInfoAsync(characterId);

        return character;
    }

    public void Log(string log)
    {
        _logger.LogInformation(log);
        ApiNotificationMessage?.Invoke(this, log);
    }
}