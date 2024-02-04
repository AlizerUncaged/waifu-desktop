using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waifu.Models;

[Table("settings")]
public class Settings
{
    [Key] public long Id { get; set; }

    /// <summary>
    /// The model this settings is for, if null, this is might be the global settings.
    /// </summary>
    public string? LocalModel { get; set; } = null;

    public SettingsTarget SettingsTarget { get; set; } = SettingsTarget.Global;

    public int GpuLayerCount { get; set; } = 35;

    public float Temperature { get; set; } = 0.6f;

    public int MaxTokens { get; set; } = 2048;

    public int TopK { get; set; } = 50;

    public float TopP { get; set; } = 0.9f;
}

public enum SettingsTarget
{
    Model,
    Global
}