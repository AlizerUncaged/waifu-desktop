using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace Waifu.Models;

[Table("channels")]
public class RoleplayCharacter
{
    [Key] public long Id { get; set; }

    public required string CharacterName { get; set; }

    public string SampleMessages { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ProfilePictureHash { get; set; } = "default.jpg";

    public string ProfilePictureFilePath => Path.Combine("Resources", "Images", ProfilePictureHash);
}