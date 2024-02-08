using CharacterAI.Client;
using Waifu.Data;
using Waifu.Models;
using Settings = Waifu.Data.Settings;

namespace Waifu.ChatHandlers;

public class CharacterAiChatHandler : IChatHandler
{
    private readonly Settings _settings;
    private readonly CharacterAiApi _characterAiApi;
    public event EventHandler<string>? CompleteMessageGenerated;

    public event EventHandler<string>? PartialMessageGenerated;

    public ChatChannel ChatChannel { get; set; }


    public CharacterAiChatHandler(Settings settings, CharacterAiApi characterAiApi)
    {
        _settings = settings;
        _characterAiApi = characterAiApi;
    }

    private bool isInitialized = false;
    private SemaphoreSlim initSemaphore = new(1);

    public async Task InitializeAsync()
    {
        await initSemaphore.WaitAsync();

        if (isInitialized)
            return;

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