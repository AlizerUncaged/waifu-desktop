using Microsoft.EntityFrameworkCore;
using Waifu.Models;

namespace Waifu.Data;

public class Messages
{
    private readonly ApplicationDbContextFactory _applicationDbContext;
    private readonly CharacterAiApi _characterAiApi;

    public Messages(ApplicationDbContextFactory applicationDbContext, CharacterAiApi characterAiApi)
    {
        _applicationDbContext = applicationDbContext;
        _characterAiApi = characterAiApi;
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(long channelId, long? currentMessageId = null,
        int maxBackwards = 15)
    {
        var messagesBeforeCurrent = await _applicationDbContext.GetDbContext().ChatMessages
            .OrderByDescending(x => x.Id)
            .Where(x => x.ChatChannel.Id == channelId && x.Id < currentMessageId)
            .Take(maxBackwards)
            .ToListAsync();

        return messagesBeforeCurrent;
    }

    public async Task<ChatMessage> AddMessageAsync(ChatMessage chatMessage)
    {
        var dbContext = _applicationDbContext.GetDbContext();

        chatMessage.ChatChannel =
            await dbContext.ChatChannels
                .FirstOrDefaultAsync(x => x.Id == chatMessage.ChatChannel.Id);

        dbContext.Add(chatMessage);

        await dbContext.SaveChangesAsync();

        return chatMessage;
    }


    public async Task<ChatChannel> GetOrCreateChannelWithCharacter(RoleplayCharacter character)
    {
        var dbContext = _applicationDbContext.GetDbContext();
        
        if (await dbContext.ChatChannels.FirstOrDefaultAsync(x =>
                x.Characters.Any(y => y.Id == character.Id)) is
            { } channel)
            return channel;

        var dbRpCharacter =
            await dbContext.RoleplayCharacters.FirstOrDefaultAsync(x => x.Id == character.Id);

        var newChannel = new ChatChannel()
        {
            Characters = { dbRpCharacter }
        };

        if (character.IsCharacterAi)
        {
            // its characterai, automatically generate a chat id
            var chatId = await _characterAiApi.GenerateNewChannelAndGetChatIdAsync(character.CharacterAiId);
            newChannel.CharacterAiHistoryId = chatId;
        }


        var channelEntity = dbContext.ChatChannels.Add(newChannel);

        await dbContext.SaveChangesAsync();

        if (character.IsCharacterAi)
        {
            // the chai character must have a greeting
            dbContext.ChatMessages.Add(new ChatMessage()
            {
                Sender = character.Id, SentByUser = false, Message = character.SampleMessages,
                ChatChannel = channelEntity.Entity
            });
        }

        return channelEntity.Entity;
    }
}