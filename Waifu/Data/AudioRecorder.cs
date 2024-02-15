using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace Waifu.Data;

using NAudio.Wave;

public class AudioRecorder
{
    private readonly ILogger<AudioRecorder> _logger;
    private readonly Hotkeys _hotkeys;
    private readonly AudioLevelCalculator _audioLevelCalculator;

    private DispatcherTimer _audioLevelTimer = new()
    {
        Interval = TimeSpan.FromSeconds(0.1)
    };

    private readonly WaveFormat _waveFormat = new(16000, 1)
    {
    };

    public bool DoRecord { get; set; } = true;

    private bool canEmit = false;

    public AudioRecorder(ILogger<AudioRecorder> logger, Hotkeys hotkeys, AudioLevelCalculator audioLevelCalculator)
    {
        _logger = logger;
        _hotkeys = hotkeys;
        _audioLevelCalculator = audioLevelCalculator;

        // _memoryStream = new WaveFileWriter(Path.GetTempFileName(), _waveInEvent.WaveFormat);
        _memoryStream = new WaveWriter(_waveInEvent.WaveFormat);

        _waveInEvent.WaveFormat = _waveFormat;
        _waveInEvent.DataAvailable += WaveInEventOnDataAvailable;

        _waveInEvent.RecordingStopped += (sender, args) => { };
        _hotkeys.HotkeyUp += (sender, s) =>
        {
            if (s == "Voice")
            {
                StopRecordingAudio();
            }
        };

        _hotkeys.HotkeyDown += (sender, s) =>
        {
            if (s == "Voice")
            {
                StartRecordingAudio();
            }
        };


        _audioLevelTimer.Tick += (sender, args) =>
        {
            canEmit = true; // we can emit at this time
        };
    }

    public event EventHandler<int> AudioRecordingStopped;


    private WaveWriter _memoryStream;

    private void WaveInEventOnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (_memoryStream.CanWrite && isRecording)
        {
            if (canEmit)
            {
                _audioLevelCalculator?.CalculateAudioLevelAndEmit(e.Buffer, e.BytesRecorded, _waveFormat,
                    TimeSpan.FromSeconds(2));
            }

            _memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
            _memoryStream.Flush();
        }
    }

    public event EventHandler<byte[]> RecordingReceived;

    private WaveInEvent _waveInEvent = new();

    private bool isRecording = false;

    public event EventHandler AudioRecordingStarted;

    public void StartRecordingAudio()
    {
        if (isRecording || !DoRecord)
            return;

        _logger.LogInformation($"Audio recording...");


        _memoryStream = new WaveWriter(_waveInEvent.WaveFormat);

        _audioLevelTimer.Start();

        _waveInEvent.StartRecording();

        isRecording = true;
        AudioRecordingStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StopRecordingAudio()
    {
        if (!isRecording)
            return;


        isRecording = false;

        _waveInEvent.StopRecording();

        _audioLevelTimer.Stop();

        _memoryStream.UpdateHeader();

        var fullData = _memoryStream.GetDataAsByteArray();

        _logger.LogInformation($"Received {fullData.Length} bytes of data in");

        RecordingReceived?.Invoke(this, fullData);


        _memoryStream.Flush();
        _memoryStream.DisposeA(true);

        _logger.LogInformation($"Recording stopped at {fullData.Length}");

        AudioRecordingStopped?.Invoke(this, fullData.Length);
    }
}