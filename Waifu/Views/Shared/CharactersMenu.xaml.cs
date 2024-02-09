using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using Waifu.Controllers;
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
    private readonly ChatAreaController _chatAreaController;

    // has to be public to be able to be accessed by the xaml frontend, so all operations of this collection
    // outside this class has to be done directly to this class lmao
    public ObservableCollection<RoleplayCharacter> RoleplayCharacters { get; set; }

    public void AddCharacter(RoleplayCharacter character)
    {
        RoleplayCharacters.Add(character);

        _logger.LogInformation($"Character {character.CharacterName} loaded to character menu");
    }

    public CharactersMenu(Characters characters,
        ILogger<CharactersMenu> logger,
        ChatAreaController chatAreaController)
    {
        _characters = characters;
        _logger = logger;
        _chatAreaController = chatAreaController;

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
        if (sender is not CharacterItem characterItem) return;

        _logger.LogInformation($"Character {characterItem.CharacterName} is clicked!");

        var rpCharacter = characterItem.RoleplayCharacter;

        if (this.GetParentType<MainArea>() is { } mainArea)
            _ = Task.Run(async () =>
            {
                // generate the chatarea context as well as the complete chat handlers
                var chatArea = await _chatAreaController.CreateChatArea(rpCharacter);

                if (chatArea is not null)
                    Dispatcher.Invoke(() => { mainArea.SetMainContent(chatArea); });
            });
    }
}