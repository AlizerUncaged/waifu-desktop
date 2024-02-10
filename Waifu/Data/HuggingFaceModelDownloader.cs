using System.IO;
using Whisper.net.Ggml;

namespace Waifu.Data;

public class HuggingFaceModelDownloader
{
    private readonly Settings _settings;
    public const string ModelFolder = "Resources/Whisper";

    public HuggingFaceModelDownloader(Settings settings)
    {
        _settings = settings;
    }

    public List<DownloadProgressData> CurrentOngoingProgress { get; set; } = new();

    public DownloadProgressData DownloadWhisperModelInBackgroundAndSetAsModel(GgmlType modelType)
    {
        var progress = new DownloadProgressData();
        var modelName = Path.Combine(ModelFolder, $"{modelType}.bin");

        _ = Task.Run(async () =>
        {
            CurrentOngoingProgress.Add(progress);

            Directory.CreateDirectory(ModelFolder);

            var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(modelType);

            var fileWriter = new FileStream(modelName, FileMode.Create, FileAccess.Write);
            
            var copiedBytes = 0L;

            var buffer = new byte[81920]; // Adjust buffer size as needed

            int bytesRead;

            while ((bytesRead = await modelStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileWriter.WriteAsync(buffer, 0, bytesRead);

                copiedBytes += bytesRead;

                progress.Report(copiedBytes); // Report progress percentage
            }

            var currentSettings = await _settings.GetOrCreateSettings();

            currentSettings.WhisperModel = modelType;

            await _settings.ClearAndAddSettings(currentSettings);

            progress.Done();

            CurrentOngoingProgress.Remove(progress);
        });

        return progress;
    }
}

public class DownloadProgressData
{
    public event EventHandler<long> ProgressPercentage;

    public event EventHandler DownloadDone;

    public void Report(long percentage)
    {
        ProgressPercentage?.Invoke(this, percentage);
    }

    public void Done()
    {
        DownloadDone?.Invoke(this, EventArgs.Empty);
    }
}