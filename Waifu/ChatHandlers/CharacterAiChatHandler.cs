using CharacterAI.Client;
using LLama.Common;
using Waifu.Data;
using Waifu.Models;
using Settings = Waifu.Data.Settings;

namespace Waifu.ChatHandlers;

public class CharacterAiChatHandler : IChatHandler
{
    private readonly Settings _settings;
    private readonly CharacterAiApi _characterAiApi;
    private readonly RoleplayCharacter _roleplayCharacter;
    private readonly Messages _messages;
    public event EventHandler<string>? CompleteMessageGenerated;

    public event EventHandler<string>? PartialMessageGenerated;

    public ChatChannel ChatChannel { get; set; }


    public CharacterAiChatHandler(Settings settings, CharacterAiApi characterAiApi,
        Messages messages, ChatChannel chatChannel, RoleplayCharacter roleplayCharacter)
    {
        _settings = settings;
        _characterAiApi = characterAiApi;
        _messages = messages;
        _roleplayCharacter = roleplayCharacter;
        ChatChannel = chatChannel;
    }

    private bool isInitialized = false;
    private SemaphoreSlim initSemaphore = new(1);

    public async Task InitializeAsync()
    {
        await initSemaphore.WaitAsync();

        if (isInitialized)
            return;

        ChatChannel = await _messages.GetOrCreateChannelWithCharacter(_roleplayCharacter);

        isInitialized = true;

        initSemaphore.Release();
    }

    public async Task<string?> SendMessageAndGetResultAsync(ChatMessage message)
    {
        if (!isInitialized)
            await InitializeAsync();


        return null;
    }
}