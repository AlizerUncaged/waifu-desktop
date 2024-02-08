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

    public async Task<IChatHandler?> GetEnabledChatService<T>() where T : IChatHandler
    {
        var chatHandler = _chatHandlers.FirstOrDefault(x => x is T);

        _logger.LogInformation($"Chat handler is {chatHandler.GetType().Name}");

        return chatHandler;
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