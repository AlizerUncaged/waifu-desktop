using System.IO;
using Microsoft.Extensions.Logging;

namespace Waifu.Data;

using NAudio.Wave;

public class AudioRecorder
{
    private readonly ILogger<AudioRecorder> _logger;

    public AudioRecorder(ILogger<AudioRecorder> logger)
    {
        _logger = logger;
        _waveInEvent.WaveFormat = new WaveFormat(44100, 1);
        _waveInEvent.DataAvailable += WaveInEventOnDataAvailable;
    }

    private MemoryStream _memoryStream = new();

    private void WaveInEventOnDataAvailable(object? sender, WaveInEventArgs e)
    {
        _memoryStream.Write(e.Buffer);
    }

    public event EventHandler<IEnumerable<byte>> RecordingReceived;

    private WaveInEvent _waveInEvent = new();

    private bool isRecording = false;

    public void StartRecordingAudio()
    {
        if (isRecording)
            return;

        _waveInEvent.StartRecording();

        isRecording = true;
    }

    public void StopRecordingAudio()
    {
        isRecording = false;

        RecordingReceived?.Invoke(this, _memoryStream.ToArray());

        // reset our memory stream
        _memoryStream.Seek(0, SeekOrigin.Begin);
        _memoryStream.SetLength(0);
    }
}