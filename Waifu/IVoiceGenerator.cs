using System.IO;

namespace Waifu;

public interface IVoiceGenerator
{
    Task<IEnumerable<byte>> GenerateVoiceAsync(string text, string voice);
}