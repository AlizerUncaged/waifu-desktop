using System.Reflection;
using Microsoft.Extensions.Logging;
using Waifu.ChatHandlers;
using Waifu.Models;

namespace Waifu.Data;

public class ChatServiceManager
{
    private readonly ILogger<ChatServiceManager> _logger;
    private readonly Settings _settings;

    public ChatServiceManager(ILogger<ChatServiceManager> logger, Settings settings)
    {
        _logger = logger;
        _settings = settings;
    }


    public Type? GetEnabledChatServiceForCharacter(RoleplayCharacter roleplayCharacter)
    {
        if (roleplayCharacter.IsCharacterAi)
            return typeof(CharacterAiChatHandler);

        return typeof(LocalLlama);
    }
}