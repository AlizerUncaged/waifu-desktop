using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Waifu.Models;

[Table("messages")]
public class ChatMessage
{
    [Key] public long Id { get; set; }

    /// <summary>
    /// If SentByUser is true, this is user ID, if not, this is character ID.
    /// </summary>
    public required long Sender { get; set; }


    public bool SentByUser { get; set; } = false;

    public ChatChannel? ChatChannel { get; set; }

    public string Message { get; set; } = string.Empty;
}