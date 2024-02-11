using System.IO;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace Waifu.Data;

using NAudio.Wave;

public class AudioRecorder
{
    private readonly ILogger<AudioRecorder> _logger;
    private readonly Hotkeys _hotkeys;

    private DispatcherTimer _audioLevelTimer = new()
    {
        Interval = TimeSpan.FromSeconds(0.1)
    };

    private readonly WaveFormat _waveFormat = new WaveFormat(16000, 1);

    public bool DoRecord { get; set; } = true;


    public AudioRecorder(ILogger<AudioRecorder> logger, Hotkeys hotkeys)
    {
        _logger = logger;
        _hotkeys = hotkeys;

        _memoryStream = new WaveFileWriter(Path.GetTempFileName(), _waveInEvent.WaveFormat);

        _waveInEvent.WaveFormat = _waveFormat;
        _waveInEvent.DataAvailable += WaveInEventOnDataAvailable;

        _waveInEvent.RecordingStopped += (sender, args) =>
        {
            logger.LogInformation($"Recording stopped.");

            _memoryStream.Flush();
            _memoryStream.Close();
            _memoryStream.Dispose();

            RecordingReceived?.Invoke(this, _memoryStream);

            //
            // var newStream = new MemoryStream();
            //
            // _memoryStream.CopyTo(newStream);
            //
            // // reset our memory stream
            // _memoryStream.Seek(0, SeekOrigin.Begin);
            // _memoryStream.SetLength(0);
            //
            //
            // AudioRecordingEnded?.Invoke(this, newStream);
        };
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
            // Get the audio samples from the memory stream
            // var audioData = _memoryStream.ToArray();
            //
            // // Calculate the audio volume level
            // var audioVolume = CalculateRmsLevel(audioData, TimeSpan.FromSeconds(1));
            //
            // _logger.LogInformation($"Audio level: {audioVolume}");
            //
            // // Raise the AudioLevelReceived event
            // AudioLevelReceived?.Invoke(this, audioVolume);
        };
    }

    public event EventHandler<double> AudioLevelReceived;

    private double CalculateRmsLevel(byte[] audioData, TimeSpan duration)
    {
        // Calculate RMS (root mean square) level
        if (audioData.Length == 0)
            return 0;

        // Calculate the number of samples corresponding to the specified duration
        int numSamplesPerSecond = _waveInEvent.WaveFormat.SampleRate;
        int numSamples = numSamplesPerSecond * (int)duration.TotalSeconds;
        int bytesPerSample = _waveInEvent.WaveFormat.BitsPerSample / 8;
        int bytesPerSecond = numSamplesPerSecond * bytesPerSample;

        // Get the starting position of the portion of audio data to process
        int startPos = Math.Max(audioData.Length - bytesPerSecond * (int)duration.TotalSeconds, 0);

        // Initialize sum for calculating RMS level
        long sum = 0;

        // Process the specified portion of audio data
        for (int i = startPos; i < audioData.Length; i += bytesPerSample)
        {
            short sample = BitConverter.ToInt16(audioData, i); // Convert 16-bit sample to short
            sum += (long)sample * sample;
        }

        // Calculate RMS level
        double rms = Math.Sqrt((double)sum / (numSamples / bytesPerSample));
        return rms;
    }


    private WaveFileWriter _memoryStream;

    private void WaveInEventOnDataAvailable(object? sender, WaveInEventArgs e)
    {
        _memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
        _memoryStream.Flush();
    }

    public event EventHandler<WaveFileWriter> RecordingReceived;

    private WaveInEvent _waveInEvent = new();

    private bool isRecording = false;

    public event EventHandler AudioRecordingStarted;

    public void StartRecordingAudio()
    {
        if (isRecording || !DoRecord)
            return;

        _logger.LogInformation($"Audio recording...");


        _memoryStream = new WaveFileWriter(Path.GetTempFileName(), _waveInEvent.WaveFormat);

        _audioLevelTimer.Start();

        _waveInEvent.StartRecording();

        isRecording = true;
        AudioRecordingStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StopRecordingAudio()
    {
        isRecording = false;

        _waveInEvent.StopRecording();

        _audioLevelTimer.Stop();
    }
}