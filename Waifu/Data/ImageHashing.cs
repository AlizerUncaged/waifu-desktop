using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using Waifu.Utilities;

namespace Waifu.Data;

/// <summary>
/// A class for image hashing, yes.
/// </summary>
public class ImageHashing
{
    public async Task<string> GetStreamHash(Stream file)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = await sha256.ComputeHashAsync(file);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public async Task<string> GetFileHash(string filePath)
    {
        using (var fileStream = File.OpenRead(filePath))
        {
            return await GetStreamHash(fileStream);
        }
    }

    public async Task<string> StoreImageAsync(string fileName)
    {
        string profilesFolder = Path.Combine(Environment.CurrentDirectory, Constants.ProfilePicturesFolder);
        Directory.CreateDirectory(profilesFolder);

        string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
        string hash = await GetFileHash(filePath);
        string fileExtension = Path.GetExtension(fileName);

        string newFileName = $"{hash}{fileExtension}";
        string destinationPath = Path.Combine(profilesFolder, newFileName);

        try
        {
            if (File.Exists(destinationPath))
                File.Delete(destinationPath);

            await FileUtilities.CopyFileAsync(filePath, destinationPath);
        }
        catch
        {
            // maybe the file is already ok
        }

        return newFileName;
    }

    public async Task<string> StoreImageFromWebAsync(string url)
    {
        using (var httpClient = new HttpClient())
        {
            // Download the image from the provided URL
            byte[] imageData = await httpClient.GetByteArrayAsync(url);

            // Save the image to a temporary file
            string tempFileName = Path.GetTempFileName();
            await File.WriteAllBytesAsync(tempFileName, imageData);

            // Store the image using the existing method
            return await StoreImageAsync(tempFileName);
        }
    }
}