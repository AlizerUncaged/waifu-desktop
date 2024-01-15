namespace Waifu.Data.ChatHandlers;

public class LlamaSharpChatHandler : IChatHandler
{
    public async Task<string?> SendMessageAsync(string message)
    {
        return null;
    }

    public event EventHandler<string>? OnMessageReceived;
}