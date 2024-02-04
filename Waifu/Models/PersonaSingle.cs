using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waifu.Models;

[Table("personas")]
public class PersonaSingle
{
    [Key] public long Id { get; set; }

    public string StageName { get; set; } = "";

    public string CharacterDescription { get; set; } = "";
}