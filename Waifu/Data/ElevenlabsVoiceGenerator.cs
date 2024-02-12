using System.IO;
using ElevenLabs;
using ElevenLabs.TextToSpeech;
using ElevenLabs.Voices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Waifu.Data;

public class ElevenlabsVoiceGenerator : IVoiceGenerator
{
    private readonly ILogger<ElevenlabsVoiceGenerator> _logger;
    private readonly Settings _settings;

    public ElevenlabsVoiceGenerator(ILogger<ElevenlabsVoiceGenerator> logger, Settings settings
    )
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task<bool> CheckElevenlabsTokenAsync()
    {
        return !string.IsNullOrWhiteSpace((await _settings.GetOrCreateSettings()).ElevenlabsApiKey);
    }

    public async Task InitializeClient()
    {
        var currentSettings = await _settings.GetOrCreateSettings();

        api = new ElevenLabsClient(currentSettings.ElevenlabsApiKey);
    }

    public IEnumerable<Voice>? VoicesCache { get; set; }

    public async Task<IEnumerable<string>> GetElevenlabsVoices()
    {
        if (api is null)
            await InitializeClient();

        var voices = await api.VoicesEndpoint.GetAllVoicesAsync();

        if (voices is not null && voices.Any())
            VoicesCache = voices;

        return voices.Select(x => x.Name);
    }

    private ElevenLabsClient? api = null;

    public async Task<IEnumerable<byte>> GenerateVoiceAsync(string text, string voice)
    {
        if (api is null)
            await InitializeClient();

        if (VoicesCache is null)
            await GetElevenlabsVoices();

        var currentSettings = await _settings.GetOrCreateSettings();


        Voice? actualVoice = null;
        if (!string.IsNullOrWhiteSpace(voice))
        {
            // voice exists
            actualVoice = VoicesCache.FirstOrDefault(x => x.Name == voice);
        }
        else
        {
            // voice not set use default
            actualVoice = VoicesCache.FirstOrDefault();
        }


        var voiceClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(text, actualVoice,
            partialClipCallback: async (partialClip) =>
            {
                // Write the incoming data to the output file stream.
                // Alternatively you can play this clip data directly.
                // await memoryStream.WriteAsync(partialClip.ClipData);
            });


        return voiceClip.ClipData.ToArray();
    }
}