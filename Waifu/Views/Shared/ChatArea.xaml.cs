using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Waifu.ChatHandlers;
using Waifu.Controllers;
using Waifu.Data;
using Waifu.Models;
using Waifu.Utilities;
using Settings = Waifu.Data.Settings;

namespace Waifu.Views.Shared;

public partial class ChatArea : UserControl
{
    private readonly RoleplayCharacter _character;
    private readonly ChatChannel _channel;
    private readonly Messages _messages;
    private readonly Settings _settings;
    private readonly Personas _personas;
    private readonly ChatAreaController _chatAreaController;
    private readonly IChatHandler _chatHandler;
    private readonly Hotkeys _hotkeys;
    private readonly AudioRecorder _audioRecorder;
    private readonly WhisperManager _whisperManager;
    private readonly CharactersMenu _charactersMenu;
    private readonly ILogger<ChatArea> _logger;

    public IChatHandler ChatHandler => _chatHandler;

    public ChatArea(RoleplayCharacter character, ChatChannel channel, Messages messages,
        Data.Settings settings, Personas personas,
        ChatAreaController chatAreaController, IChatHandler chatHandler, Hotkeys hotkeys, AudioRecorder audioRecorder,
        WhisperManager whisperManager, CharactersMenu charactersMenu, ILogger<ChatArea> logger)
    {
        _character = character;
        _channel = channel;
        _messages = messages;
        _settings = settings;
        _personas = personas;
        _chatAreaController = chatAreaController;
        _chatHandler = chatHandler;
        _hotkeys = hotkeys;
        _audioRecorder = audioRecorder;
        _whisperManager = whisperManager;
        _charactersMenu = charactersMenu;
        _logger = logger;

        InitializeComponent();

        _whisperManager.DoTranscribe = true;

        _whisperManager.TranscribeFinished += WhisperManagerOnTranscribeFinished;
        _whisperManager.Transcribing += WhisperManagerOnTranscribing;
        audioRecorder.AudioRecordingStarted += AudioRecorderOnAudioRecordingStarted;

        audioRecorder.AudioLevelReceived += AudioRecorderOnAudioLevelReceived;
    }

    private void WhisperManagerOnTranscribing(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            SendButton.IsEnabled = false;
            SendButtonText.Text = "Transcribing";
        });
    }

    private double _maxVoiceLevel = 100;

    private void AudioRecorderOnAudioLevelReceived(object? sender, double e)
    {
        Dispatcher.Invoke(() =>
        {
            VoiceBorder.Opacity = e / _maxVoiceLevel;

            if (e > _maxVoiceLevel)
                _maxVoiceLevel = e;
        });
    }

    private void AudioRecorderOnAudioRecordingStarted(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            SendButton.IsEnabled = false;
            SendButtonText.Text = "Listening";
        });
    }

    private void WhisperManagerOnTranscribeFinished(object? sender, string e)
    {
        Dispatcher.Invoke(() =>
        {
            SendButton.IsEnabled = true;
            SendButtonText.Text = "Send";
            VoiceBorder.Opacity = 0;
            MessageInput.Text = e;
            SendMessageFromUi();
        });
    }

    public Hotkey VoiceHotkey { get; set; }
    public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new();

    public RoleplayCharacter RoleplayCharacter => _character;
    public ChatChannel ChatChannel => _channel;

    public event EventHandler<string> MessageSend;

    public void SendMessageFromUi()
    {
        MessageSend?.Invoke(this, MessageInput.Text);

        // clear text
        MessageInput.Text = string.Empty;
    }

    private void SendMessageButton(object sender, RoutedEventArgs e)
    {
        SendMessageFromUi();
    }

    private void TextBoxTextChanged(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Enter)
            SendMessageFromUi();
    }

    public long CurrentMessageId { get; set; } = long.MaxValue;


    private void ChatAreaLoaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            VoiceHotkey = await _hotkeys.GetOrAddHotkeyAsync("Voice", Key.LeftAlt);

            var currentMessages = await _messages.GetMessagesAsync(_channel.Id, CurrentMessageId);

            if (currentMessages is null || !currentMessages.Any())
            {
                // we've probably reached the first message
                return;
            }

            CurrentMessageId = currentMessages.MinBy(x => x.Id).Id;

            Dispatcher.Invoke(() =>
            {
                VoiceHotkeyText.Text = VoiceHotkey.VirtualKeyCodes.ToHotkeyString();

                foreach (var chatMessage in currentMessages.OrderByDescending(x => x.Id))
                    AddChatBasedOnIdLocation(chatMessage);
            });
        });

        _chatAreaController.MessageFromCurrentUser += ChatAreaControllerOnMessageFromCurrentUser;
    }

    private void ChatAreaControllerOnMessageFromCurrentUser(object? sender, ChatMessage e)
    {
        Dispatcher.Invoke(() => { ChatMessages.Add(e); });
    }

    public void AddChatBasedOnIdLocation(ChatMessage chatMessage)
    {
        // only add if the chat channel are the same

        if (chatMessage.ChatChannel is null || _channel.Id != chatMessage.ChatChannel.Id)
        {
            return;
        }

        Dispatcher.Invoke(() =>
        {
            var indexToInsert = ChatMessages
                .Select((msg, index) => new { Message = msg, Index = index })
                .OrderBy(item => item.Message.Id)
                .TakeWhile(item => item.Message.Id < chatMessage.Id)
                .LastOrDefault()?.Index + 1 ?? 0;

            if (indexToInsert < 0)
                indexToInsert = 0;

            ChatMessages.Insert(indexToInsert, chatMessage);

            // goofy ahh ui update
            ChatScroll.ScrollToEnd();

            UpdateLayout();

            ChatScroll.ScrollToEnd();
        });
    }

    private void ChatAreaDisposed(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation($"ChatArea {this.GetHashCode()} is being disposed!");

        _whisperManager.Transcribing -= WhisperManagerOnTranscribing;
        _whisperManager.TranscribeFinished -= WhisperManagerOnTranscribeFinished;
        _audioRecorder.AudioRecordingStarted -= AudioRecorderOnAudioRecordingStarted;
        _audioRecorder.AudioLevelReceived -= AudioRecorderOnAudioLevelReceived;
        _chatAreaController.MessageFromCurrentUser -= ChatAreaControllerOnMessageFromCurrentUser;
    }
}