using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waifu.Models;

[Table("channels")]
public class ChatChannel
{
    [Key] public long Id { get; set; }

    public required RoleplayCharacter Character { get; set; }
}