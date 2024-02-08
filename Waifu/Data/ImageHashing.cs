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

    // New method
    public async Task<string> StoreImageAsync(string fileName)
    {
        // Ensure the "Profiles" folder exists
        string profilesFolder = Path.Combine(Environment.CurrentDirectory, Constants.ProfilePicturesFolder);
        Directory.CreateDirectory(profilesFolder);

        // Generate the hash and get the file extension
        string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
        string hash = await GetFileHash(filePath);
        string fileExtension = Path.GetExtension(fileName);

        // Construct the new file name with hash and original file extension
        string newFileName = $"{hash}{fileExtension}";
        string destinationPath = Path.Combine(profilesFolder, newFileName);

        if (File.Exists(destinationPath))
            File.Delete(destinationPath);

        // Copy the file to the "Profiles" folder with the new name
        await FileUtilities.CopyFileAsync(filePath, destinationPath);

        // Return only the new filename (not the full path)
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