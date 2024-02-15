using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Waifu.Data.Args;

namespace Waifu.Data;

public class AudioLevelCalculator
{
    private readonly ILogger<AudioLevelCalculator> _logger;

    public AudioLevelCalculator(ILogger<AudioLevelCalculator> logger)
    {
        _logger = logger;
    }

    public event EventHandler<AudioLevelData> AudioLevelCalculated;


    public void CalculateAudioLevelAndEmit(byte[] audioData, int bytes, WaveFormat waveFormat, TimeSpan duration,
        string target = "Mic")
    {
        // Calculate RMS (root mean square) level
        if (audioData.Length == 0)
            return;

        // Calculate the number of samples corresponding to the specified duration
        int numSamplesPerSecond = waveFormat.SampleRate;
        int numSamples = numSamplesPerSecond * (int)duration.TotalSeconds;
        int bytesPerSample = waveFormat.BitsPerSample / 8;
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

        AudioLevelCalculated?.Invoke(this, new AudioLevelData()
        {
            AudioTarget = target, AudioLevel = rms
        });

        _logger.LogInformation($"Audio level {rms} for {target}");
    }
}