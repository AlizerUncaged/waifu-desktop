using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace Waifu.Data;

public class Settings
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Settings(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }


    public IEnumerable<string> GetModelsOnDirectory()
    {
        Directory.CreateDirectory(Constants.ModelsFolder);

        var filesInModelsFolder = Directory.GetFiles(Constants.ModelsFolder, "*.gguf", SearchOption.AllDirectories);
        List<string> modelNames = new();

        foreach (var file in filesInModelsFolder)
            modelNames.Add(file);

        return modelNames;
    }

    public async Task ClearAndAddSettings(Waifu.Models.Settings settings)
    {
        _applicationDbContext.Settings.RemoveRange(await _applicationDbContext.Settings.ToListAsync());

        await _applicationDbContext.SaveChangesAsync();
        
        _applicationDbContext.Settings.Add(settings);

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<Waifu.Models.Settings> GetOrCreateSettings()
    {
        var existingSettings = await _applicationDbContext.Settings.FirstOrDefaultAsync(x => x.LocalModel != null);

        if (existingSettings is { })
            return existingSettings;

        var modelsInDirectory = GetModelsOnDirectory();


        var settings = new Waifu.Models.Settings()
        {
            LocalModel = modelsInDirectory.FirstOrDefault()
        };

        _applicationDbContext.Settings.Add(settings);

        await _applicationDbContext.SaveChangesAsync();

        return settings;
    }
}