using System.Windows;
using Autofac;
using Waifu.Data;
using Waifu.Models;
using Waifu.Views.Shared;
using Settings = Waifu.Data.Settings;

namespace Waifu.Controllers;

public class ChatAreaController
{
    private readonly Messages _messages;
    private readonly ChatServiceManager _chatServiceManager;
    private readonly Settings _settings;
    private readonly Personas _personas;
    private readonly ILifetimeScope _lifetimeScope;

    public ChatAreaController(Messages messages,
        ChatServiceManager chatServiceManager,
        Settings settings,
        Personas personas,
        ILifetimeScope lifetimeScope)
    {
        _messages = messages;
        _chatServiceManager = chatServiceManager;
        _settings = settings;
        _personas = personas;
        _lifetimeScope = lifetimeScope;
    }

    public async Task<ChatArea> CreateChatArea(RoleplayCharacter roleplayCharacter)
    {
        var channelWithCharacter = await _messages.GetOrCreateChannelWithCharacter(roleplayCharacter);

        var chatAreaScope = _lifetimeScope.BeginLifetimeScope(x =>
        {
            x.RegisterInstance(roleplayCharacter).AsSelf().SingleInstance();
            x.RegisterInstance(channelWithCharacter).AsSelf().SingleInstance();
        });

        ChatArea chatArea = default;

        Application.Current.Dispatcher.Invoke(() =>
        {
            chatArea = chatAreaScope.Resolve<ChatArea>();

            chatArea.MessageSend += ChatAreaOnMessageSend;
        });

        return chatArea;
    }

    /// <summary>
    /// Callback for message from current user.
    /// </summary>
    public event EventHandler<ChatMessage> MessageFromCurrentUser;

    private void ChatAreaOnMessageSend(object? sender, string e)
    {
        if (sender is not ChatArea chatArea) return;

        var stringMessageContent = e.Trim();

        var message = new ChatMessage()
        {
            ChatChannel = chatArea.ChatChannel, Sender = -1, Message = stringMessageContent, SentByUser = true
        };

        _ = Task.Run(async () =>
        {
            var dbMessage = await _messages.AddMessageAsync(message);

            MessageFromCurrentUser?.Invoke(this, dbMessage);
        });
    }
}