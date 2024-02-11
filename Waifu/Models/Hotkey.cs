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

    [NotMapped] // dont map this to db jesus
    public List<Key> VirtualKeyCodes { get; set; }

    [Column("VirtualKeyCodes")]
    public string VirtualKeyCodesJson
    {
        get => JsonConvert.SerializeObject(VirtualKeyCodes);
        set => VirtualKeyCodes = JsonConvert.DeserializeObject<List<Key>>(value) ?? new List<Key>();
    }
}