using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vulkan;
using Waifu.Data;
using Waifu.Models;
using Waifu.Views.Shared;
using Waifu.Views.Shared.Popups;

namespace Waifu.Views.Index;

public partial class MainArea : UserControl
{
    private readonly AtarashiCharacter _atarashiCharacter;
    private readonly Characters _characters;
    private readonly CharactersMenu _charactersMenu;
    private readonly AtarashiCharacterAi _atarashiCharacterAi;
    private readonly CharacterAiApi _characterAiApi;

    public MainArea(AtarashiCharacter atarashiCharacter, Characters characters, CharactersMenu charactersMenu,
        AtarashiCharacterAi atarashiCharacterAi, CharacterAiApi characterAiApi)
    {
        _atarashiCharacter = atarashiCharacter;
        _characters = characters;
        _charactersMenu = charactersMenu;
        _atarashiCharacterAi = atarashiCharacterAi;
        _characterAiApi = characterAiApi;

        InitializeComponent();
    }

    /// <summary>
    /// Set's the current main content of the window.
    /// </summary>
    public void OpenDialog<T>(T child) where T : FrameworkElement, IPopup
    {
        Dispatcher.Invoke(() =>
        {
            DialogArea.Children.Clear();

            DialogArea.Children.Add(child);

            child.CloseTriggered += (sender, args) => { PopupDialogs.IsOpen = false; };

            PopupDialogs.IsOpen = true;
        });
    }

    public void SetMainContent<T>(T child) where T : FrameworkElement
    {
        MainAreaContent.Children.Clear();

        MainAreaContent.Children.Add(child);
    }

    private void NewCharacter(object sender, RoutedEventArgs routedEventArgs)
    {
        if (FindResource("AddCharacterContextMenu") is ContextMenu contextMenu)
        {
            contextMenu.PlacementTarget = sender as FrameworkElement;
            contextMenu.IsOpen = true;
        }
    }

    private void MainAreaLoaded(object sender, RoutedEventArgs e)
    {
        if (CharactersMenuControl.Children.Count <= 0)
            CharactersMenuControl.Children.Add(_charactersMenu);

        // start showing characters
        _ = Task.Run(async () =>
        {
            var roleplayCharacters = await _characters.GetAllRoleplayCharactersAsync();

            Dispatcher.Invoke(() =>
            {
                foreach (var roleplayCharacter in roleplayCharacters)
                    _charactersMenu?.AddCharacter(roleplayCharacter);
            });
        });
    }

    private void NewLocalCharacter(object sender, RoutedEventArgs e)
    {
        OpenDialog(_atarashiCharacter);
    }

    private void NewCharacterAi(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            if (await _characterAiApi.CheckCharacterAiToken() is false)
                return;
            
            OpenDialog(_atarashiCharacterAi);
        });
    }
}