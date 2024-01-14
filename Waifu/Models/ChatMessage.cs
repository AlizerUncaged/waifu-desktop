using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waifu.Models;

[Table("messages")]
public class ChatMessage
{
    [Key] public long Id { get; set; }

    public required long Sender { get; set; }

    public bool SentByUser { get; set; } = false;

    public ChatChannel? ChatChannel { get; set; }

    public string Message { get; set; } = string.Empty;
}