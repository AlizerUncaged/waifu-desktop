using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Waifu.ChatHandlers;
using Waifu.Data;
using Waifu.Models;
using Settings = Waifu.Data.Settings;

namespace Waifu.Views.Shared;

public partial class ChatArea : UserControl
{
    private readonly RoleplayCharacter _character;
    private readonly ChatChannel _channel;
    private readonly Messages _messages;
    private readonly ChatServiceManager _chatServiceManager;
    private readonly Settings _settings;
    private readonly Personas _personas;

    public ChatArea(RoleplayCharacter character, ChatChannel channel, Messages messages, ChatServiceManager chatServiceManager,
        Data.Settings settings, Personas personas)
    {
        _character = character;
        _channel = channel;
        _messages = messages;
        _chatServiceManager = chatServiceManager;
        _settings = settings;
        _personas = personas;

        InitializeComponent();
    }

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

    public LocalLlama LocalLlama { get; set; }

    public long CurrentMessageId { get; set; } = long.MaxValue;

    private void ChatAreaLoaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            var currentMessages = await _messages.GetMessagesAsync(_channel.Id, CurrentMessageId);

            if (currentMessages is null || !currentMessages.Any())
            {
                // we've probably reached the first message
                return;
            }

            CurrentMessageId = currentMessages.MinBy(x => x.Id).Id;

            // load the llama model
            // LocalLlama =
            //     await _chatServiceManager.LoadLlamaAndResult(_channel,
            //         await _settings.GetOrCreateSettings(),
            //         await _personas.GetOrCreatePersona(), _character);

            Dispatcher.Invoke(() =>
            {
                foreach (var chatMessage in currentMessages.OrderByDescending(x => x.Id))
                    AddChatBasedOnIdLocation(chatMessage);
            });
        });
    }

    void AddChatBasedOnIdLocation(ChatMessage chatMessage)
    {
        int indexToInsert = ChatMessages
            .Select((msg, index) => new { Message = msg, Index = index })
            .OrderBy(item => item.Message.Id)
            .TakeWhile(item => item.Message.Id < chatMessage.Id)
            .LastOrDefault()?.Index + 1 ?? 0;

        if (indexToInsert < 0)
            indexToInsert = 0;

        ChatMessages.Insert(indexToInsert, chatMessage);

        // Scroll to the end after adding a new message
        ChatScroll.ScrollToEnd();

        // Ensure the layout is updated before scrolling
        UpdateLayout();

        // Scroll to the end again to ensure it's accurate
        ChatScroll.ScrollToEnd();
    }
}