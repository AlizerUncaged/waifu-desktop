using Microsoft.Extensions.Logging;
using Waifu.ChatHandlers;
using Waifu.Models;

namespace Waifu.Data;

public class Llama
{
    private readonly ILogger<Llama> _logger;

    public Llama(ILogger<Llama> logger)
    {
        _logger = logger;
    }

    public async Task<LocalLlama> LoadLlamaAndResult(ChatChannel chatChannel, Waifu.Models.Settings settings,
        PersonaSingle personaSingle,
        RoleplayCharacter roleplayCharacter)
    {
        var llamaObj = new LocalLlama(chatChannel, settings, personaSingle, roleplayCharacter);

        _ = llamaObj.InitializeAsync();

        return llamaObj;
    }
}