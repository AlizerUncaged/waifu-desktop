using Microsoft.Extensions.Logging;
using Waifu.ChatHandlers;
using Waifu.Models;

namespace Waifu.Data;

public class ChatServiceManager
{
    private readonly ILogger<ChatServiceManager> _logger;
    private readonly Settings _settings;
    private readonly IEnumerable<IChatHandler> _chatHandlers;

    public ChatServiceManager(ILogger<ChatServiceManager> logger, Settings settings,
        IEnumerable<IChatHandler> chatHandlers)
    {
        _logger = logger;
        _settings = settings;
        _chatHandlers = chatHandlers;
    }

    public async Task<IChatHandler?> GetEnabledChatService()
    {
        var settings = await _settings.GetOrCreateSettings();

        if (settings.UseCharacterAi)
            return _chatHandlers.FirstOrDefault(x => x is CharacterAi);


        return null;
    }

    // public async Task<LocalLlama> LoadLlamaAndResult(ChatChannel chatChannel, Waifu.Models.Settings settings,
    //     PersonaSingle personaSingle,
    //     RoleplayCharacter roleplayCharacter)
    // {
    //     var llamaObj = new LocalLlama(chatChannel, settings, personaSingle, roleplayCharacter);
    //
    //     _ = llamaObj.InitializeAsync();
    //
    //     return llamaObj;
    // }
}