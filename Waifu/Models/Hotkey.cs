using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Waifu.Models;

[Table("hotkeys")]
public class Hotkey
{
    [Key] public long Id { get; set; }

    public string Name { get; set; }

    [NotMapped] // This attribute ensures EF doesn't try to map this property to the database
    public List<Key> VirtualKeyCodes { get; set; }

    // This property will be mapped to the database
    [Column("VirtualKeyCodes")]
    public string VirtualKeyCodesJson
    {
        get => JsonConvert.SerializeObject(VirtualKeyCodes);
        set => VirtualKeyCodes = JsonConvert.DeserializeObject<List<Key>>(value) ?? new List<Key>();
    }
}