using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waifu.Models;

[Table("models")]
public class LocalLlamaModel
{
    [Key] public long Id { get; set; }

    public string? Source { get; set; }

    public required string Name { get; set; }

    public string? ModelHash { get; set; }

    public string? FilePath { get; set; }
}