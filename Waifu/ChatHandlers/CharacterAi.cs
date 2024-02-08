using Waifu.Models;

namespace Waifu.ChatHandlers;

public class CharacterAi : IChatHandler
{
    public event EventHandler<string>? CompleteMessageGenerated;
    public event EventHandler<string>? PartialMessageGenerated;
    public ChatChannel ChatChannel { get; set; }
    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string?> SendMessageAndGetResultAsync(string message)
    {
        throw new NotImplementedException();
    }
}