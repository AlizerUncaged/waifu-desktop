using Microsoft.EntityFrameworkCore;
using Waifu.Models;

namespace Waifu.Data;

public class Messages
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Messages(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(long channelId, long? currentMessageId = null,
        int maxBackwards = 15)
    {
        var messagesBeforeCurrent = await _applicationDbContext.ChatMessages
            .OrderByDescending(x => x.Id)
            .Where(x => x.ChatChannel.Id == channelId && x.Id < currentMessageId)
            .Take(maxBackwards)
            .ToListAsync();

        return messagesBeforeCurrent;
    }

    public async Task<ChatMessage> AddMessageAsync(ChatMessage chatMessage)
    {
        chatMessage.ChatChannel =
            await _applicationDbContext.ChatChannels.FirstOrDefaultAsync(x => x.Id == chatMessage.ChatChannel.Id);
        _applicationDbContext.ChatMessages.Add(chatMessage);

        await _applicationDbContext.SaveChangesAsync();

        return chatMessage;
    }

    public async Task<ChatChannel> GetOrCreateChannelWithCharacter(RoleplayCharacter character)
    {
        if (await _applicationDbContext.ChatChannels.FirstOrDefaultAsync(x =>
                x.Characters.Any(y => y.Id == character.Id)) is
            { } channel)
            return channel;

        var dbRpCharacter =
            await _applicationDbContext.RoleplayCharacters.FirstOrDefaultAsync(x => x.Id == character.Id);

        var channelEntity = _applicationDbContext.ChatChannels.Add(new ChatChannel()
        {
            Characters = { dbRpCharacter }
        });

        await _applicationDbContext.SaveChangesAsync();

        return channelEntity.Entity;
    }
}