using System.IO;
using System.Windows.Forms.VisualStyles;
using NAudio.Extras;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Waifu.Data;

public class AudioPlayer
{
    private readonly Settings _settings;
    private readonly AudioLevelCalculator _audioLevelCalculator;

    public AudioPlayer(Settings settings, AudioLevelCalculator audioLevelCalculator)
    {
        _settings = settings;
        _audioLevelCalculator = audioLevelCalculator;
    }

    public static List<WaveOutEvent> WaveOutEvents = new();

    public static void StopWaveOutEvent(WaveOutEvent waveOutEvent)
    {
        waveOutEvent.Stop();
        if (WaveOutEvents.Contains(waveOutEvent))
            WaveOutEvents.Remove(waveOutEvent);
    }

    public async Task PlayMp3FromByteArrayAsync(byte[] mp3Data, bool stopOtherWaveOutEvents = true)
    {
        if (stopOtherWaveOutEvents)
            foreach (var waveOut in WaveOutEvents)
            {
                StopWaveOutEvent(waveOut);
            }

        var currentSettings = await _settings.GetOrCreateSettings();
        int audioOutDeviceId = currentSettings.AudioPlayerDeviceId;
        using (MemoryStream mp3Stream = new MemoryStream(mp3Data))
        {
            using (Mp3FileReader mp3FileReader = new Mp3FileReader(mp3Stream))
            {
                using (var wavStream = WaveFormatConversionStream.CreatePcmStream(mp3FileReader))
                {
                    using (var baStream = new BlockAlignReductionStream(wavStream))
                    {  
                        var meteringProvider = new MeteringSampleProvider(baStream.ToSampleProvider());

                        meteringProvider.StreamVolume += (sender, e) =>
                        {
                            _audioLevelCalculator.CalculateAudioLevelAndEmit(
                                e.MaxSampleValues.Max());
                        };
                        
                        using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                        {
                            waveOut.DeviceNumber = audioOutDeviceId >= 0 ? audioOutDeviceId : -1;

                          

                            waveOut.Init(meteringProvider);
                            waveOut.Play();

                            await Task.Run(async () =>
                            {
                                while (waveOut.PlaybackState == PlaybackState.Playing)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(1));
                                }
                            });
                        }
                    }
                }
            }
        }
    }
}