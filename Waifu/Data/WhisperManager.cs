using System.IO;
using System.Text;
using Humanizer;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Whisper.net;
using Whisper.net.Ggml;

namespace Waifu.Data;

public class WhisperManager
{
    private readonly Settings _settings;
    private readonly ILogger<WhisperManager> _logger;
    private readonly AudioRecorder _audioRecorder;

    public WhisperManager(Settings settings, ILogger<WhisperManager> logger, AudioRecorder audioRecorder)
    {
        _settings = settings;
        _logger = logger;
        _audioRecorder = audioRecorder;

        audioRecorder.RecordingReceived += (sender, stream) =>
        {
            if (DoTranscribe)
                _ = ProcessRecordingFromBytes(stream);
        };
    }

    public Dictionary<GgmlType, WhisperProcessor> WhisperFactories { get; set; } =
        new();

    public bool DoTranscribe { get; set; } = false;

    public event EventHandler<string> TranscribeFinished;

    public async Task<string> ProcessRecordingFromBytes(WaveFileWriter recordingStream)
    {
        var currentSettings = await _settings.GetOrCreateSettings();
        var model = currentSettings.WhisperModel;
        WhisperProcessor whisperFactory;

        if (WhisperFactories.TryGetValue(model, out var foundWhisperFactory))
        {
            whisperFactory = foundWhisperFactory;
        }
        else
        {
            var modelLocation = Path.Combine(WhisperHuggingFaceModelDownloader.ModelFolder, $"{model}.bin");
            modelLocation = Path.GetFullPath(modelLocation);

            whisperFactory =
                WhisperFactory.FromPath(modelLocation)
                    .CreateBuilder().WithLanguage("auto").Build();

            WhisperFactories.Add(model, whisperFactory);
        }

        StringBuilder stringBuilder = new();

        await foreach (var result in whisperFactory.ProcessAsync(File.Open(recordingStream.Filename, FileMode.Open,
                           FileAccess.Read)))
        {
            stringBuilder.Append(result.Text);
        }

        var resultComplete = stringBuilder.ToString();

        _logger.LogDebug($"Read from {recordingStream.Length.Bytes().Humanize()} of recording: {resultComplete}");

        TranscribeFinished?.Invoke(this, resultComplete);

        return resultComplete;
    }
}