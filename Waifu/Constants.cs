using System.IO;

namespace Waifu;

public static class Constants
{
    public const string LocalResourcesFolder = "Resources";

    public const string ProfilePicturesFolder = $"{LocalResourcesFolder}\\Images";

    public const string ModelsFolder = $"{LocalResourcesFolder}\\Models";

    public const string DatabasePath = $"{LocalResourcesFolder}\\database.db";

    public const string DefaultProfilePicture = $".\\{ProfilePicturesFolder}\\default.jpg";

    public static readonly string DataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Alizer", "Waifu");
}