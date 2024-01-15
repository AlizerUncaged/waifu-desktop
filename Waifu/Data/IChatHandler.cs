namespace Waifu.Data;

public interface IChatHandler
{
    /// <summary>
    /// Sends a message to the chat.
    /// </summary>
    /// <returns>A string of the complete response from this chat handler.</returns>
    Task<string?> SendMessageAsync(string message);

    /// <summary>
    /// Gets called whenever a message is received from the other side.
    /// </summary>
    event EventHandler<string> OnMessageReceived;
}