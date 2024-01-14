using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waifu.Models;

[Table("channels")]
public class ChatMessage
{
    [Key] public long Id { get; set; }

    public required long Sender { get; set; }

    public required long TargetChannelId { get; set; }

    public string Message { get; set; } = string.Empty;
}