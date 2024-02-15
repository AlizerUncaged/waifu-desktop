using System.Windows;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Waifu.ChatHandlers;
using Waifu.Data;
using Waifu.Models;
using Waifu.Views.Shared;
using Settings = Waifu.Data.Settings;

namespace Waifu.Controllers;

public class ChatAreaController
{
    private readonly Messages _messages;
    private readonly Settings _settings;
    private readonly Personas _personas;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ChatServiceManager _chatServiceManager;
    private readonly ILogger<ChatAreaController> _logger;
    private readonly IVoiceGenerator _voiceGenerator;
    private readonly AudioRecorder _audioRecorder;
    private readonly EventMaster _eventMaster;

    public ChatAreaController(Messages messages,
        Settings settings,
        Personas personas,
        ILifetimeScope lifetimeScope,
        ChatServiceManager chatServiceManager,
        ILogger<ChatAreaController> logger,
        IVoiceGenerator voiceGenerator,
        AudioRecorder audioRecorder, EventMaster eventMaster)
    {
        _messages = messages;
        _settings = settings;
        _personas = personas;
        _lifetimeScope = lifetimeScope;
        _chatServiceManager = chatServiceManager;
        _logger = logger;
        _voiceGenerator = voiceGenerator;
        _audioRecorder = audioRecorder;
        _eventMaster = eventMaster;
    }


    public async Task<ChatArea?> CreateChatArea(RoleplayCharacter roleplayCharacter)
    {
        var chatHandlerForUserType = _chatServiceManager.GetEnabledChatServiceForCharacter(roleplayCharacter);

        if (chatHandlerForUserType is null)
        {
            _eventMaster.TriggerInfo("No chat service available for character");
            return null;
        }

        if (chatHandlerForUserType == typeof(LocalLlama))
        {
            _eventMaster.TriggerInfo("Coming soon");
            return null;
        }

        var channelWithCharacter = await _messages.GetOrCreateChannelWithCharacter(roleplayCharacter);
        var defaultPersona = await _personas.GetOrCreatePersona();
        var chatAreaScope = _lifetimeScope.BeginLifetimeScope(x =>
        {
            x.RegisterInstance(roleplayCharacter)
                .AsSelf()
                .SingleInstance();

            x.RegisterInstance(channelWithCharacter)
                .AsSelf()
                .SingleInstance();

            x.RegisterInstance(defaultPersona)
                .AsSelf()
                .SingleInstance();

            // chat handlers
            x.RegisterType(chatHandlerForUserType).As<IChatHandler>().SingleInstance().AsSelf();

            x.RegisterType<ChatServiceManager>()
                .AsSelf()
                .SingleInstance();
        });


        var chatHandlerForUser = chatAreaScope.Resolve<IChatHandler>();

        ChatArea? chatArea = default;

        Application.Current.Dispatcher.Invoke(() =>
        {
            chatArea = chatAreaScope.Resolve<ChatArea>();

            chatArea.Unloaded += (sender, args) => { };

            chatHandlerForUser.CompleteMessageGenerated += (sender, message) =>
            {
                chatArea.AddChatBasedOnIdLocation(message);
            };

            chatArea.MessageSend += ChatAreaOnMessageSend;
        });

        _logger.LogInformation(
            $"Initialized chat session.{Environment.NewLine}Channel: {JsonConvert.SerializeObject(channelWithCharacter)}{Environment.NewLine}Character: {JsonConvert.SerializeObject(roleplayCharacter)}{Environment.NewLine}Persona: {JsonConvert.SerializeObject(defaultPersona)}{Environment.NewLine}Chat handler: {chatHandlerForUserType.FullName}");

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
            // process the message to the character ai parallel
            _ = chatArea.ChatHandler.SendMessageAndGetResultAsync(message);

            // save the user's sent message to the ui
            MessageFromCurrentUser?.Invoke(this, message);
        });
    }
}