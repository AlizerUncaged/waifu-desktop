using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace Waifu.Data;

public class Settings
{
    private readonly ApplicationDbContextFactory _applicationDbContext;

    public Settings(ApplicationDbContextFactory applicationDbContext)
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
        var dbContext = _applicationDbContext.GetDbContext();

        dbContext.Settings.RemoveRange(await dbContext.Settings.ToListAsync());

        await dbContext.SaveChangesAsync();

        dbContext.Settings.Add(settings);

        await dbContext.SaveChangesAsync();
    }

    public async Task<Waifu.Models.Settings> GetOrCreateSettings()
    {
        var dbContext = _applicationDbContext.GetDbContext();
        var existingSettings = await dbContext.Settings.FirstOrDefaultAsync(x => x.LocalModel != null);

        if (existingSettings is { })
            return existingSettings;

        var modelsInDirectory = GetModelsOnDirectory();


        var settings = new Waifu.Models.Settings()
        {
            LocalModel = modelsInDirectory.FirstOrDefault()
        };

        dbContext.Settings.Add(settings);

        await dbContext.SaveChangesAsync();

        return settings;
    }
}