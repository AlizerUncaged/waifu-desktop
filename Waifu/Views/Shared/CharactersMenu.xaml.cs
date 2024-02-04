using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using Waifu.Data;
using Waifu.Models;
using Waifu.Utilities;
using Waifu.Views.Index;
using Settings = Waifu.Data.Settings;

namespace Waifu.Views.Shared;

/// <summary>
/// Creates a scrollable list of characters.
/// </summary>
public partial class CharactersMenu : UserControl
{
    private readonly Characters _characters;
    private readonly ILogger<CharactersMenu> _logger;
    private readonly Messages _messages;
    private readonly Llama _llama;
    private readonly Personas _personas;
    private readonly Settings _settings;

    // has to be public to be able to be accessed by the xaml frontend, so all operations of this collection
    // outside this class has to be done directly to this class lmao
    public ObservableCollection<RoleplayCharacter> RoleplayCharacters { get; set; }

    public void AddCharacter(RoleplayCharacter character)
    {
        RoleplayCharacters.Add(character);

        _logger.LogInformation($"Character {character.CharacterName} loaded to character menu");
    }

    public CharactersMenu(Characters characters, ILogger<CharactersMenu> logger, Messages messages, Llama llama,
        Personas personas, Settings settings)
    {
        _characters = characters;
        _logger = logger;
        _messages = messages;
        _llama = llama;
        _personas = personas;
        _settings = settings;

        RoleplayCharacters =
            new ObservableCollection<RoleplayCharacter>(Enumerable.Empty<RoleplayCharacter>());

        InitializeComponent();
    }

    public RoleplayCharacter? ActiveCharacter { get; set; }

    private void CharactersMenuLoaded(object sender, RoutedEventArgs e)
    {
        _characters.OnCharacterAdded += (o, character) => { Dispatcher.Invoke(() => AddCharacter(character)); };

        _characters.OnCharacterRemoved += (o, character) =>
        {
            Dispatcher.Invoke(() => RoleplayCharacters.Remove(character));
        };

        ActiveCharacter = RoleplayCharacters.FirstOrDefault();
    }

    private void CharacterClicked(object? sender, RoleplayCharacter? e)
    {
        if (sender is CharacterItem characterItem)
        {
            _logger.LogInformation($"Character {characterItem.CharacterName} is clicked!");

            var rpCharacter = characterItem.RoleplayCharacter;

            if (this.GetParentType<MainArea>() is { } mainArea && characterItem.RoleplayCharacter is not null)
                _ = Task.Run(async () =>
                {
                    var settings = await _settings.GetOrCreateSettings();
                    
                    if (settings is null)
                        return;
                    
                    var channelWithCharacter = await _messages.GetOrCreateChannelWithCharacter(rpCharacter);

                    Dispatcher.Invoke(() =>
                    {
                        var chatArea = new ChatArea(characterItem.RoleplayCharacter,
                            channelWithCharacter, _messages, _llama, _settings, _personas);

                        chatArea.MessageSend += (o, s) =>
                        {
                            var message = new ChatMessage()
                            {
                                ChatChannel = chatArea.ChatChannel, Sender = -1, Message = s.Trim(), SentByUser = true
                            };

                            _ = Task.Run(async () => { message = await _messages.AddMessageAsync(message); });
                        };
                        mainArea.SetMainContent(chatArea);
                    });
                });
        }
    }
}