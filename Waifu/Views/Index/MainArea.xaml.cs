using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autofac;
using Vulkan;
using Waifu.Data;
using Waifu.Models;
using Waifu.Views.Shared;
using Waifu.Views.Shared.Popups;

namespace Waifu.Views.Index;

public partial class MainArea : UserControl
{
    private readonly Characters _characters;
    private readonly CharactersMenu _charactersMenu;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly CharacterAiApi _characterAiApi;

    public MainArea(Characters characters, CharactersMenu charactersMenu,
        ILifetimeScope lifetimeScope, CharacterAiApi characterAiApi)
    {
        _characters = characters;
        _charactersMenu = charactersMenu;
        _lifetimeScope = lifetimeScope;
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
        OpenDialog(_lifetimeScope.Resolve<AtarashiCharacter>());
    }

    private void NewCharacterAi(object sender, RoutedEventArgs e)
    {
        // new isntance of this user control always!!!!
        var atarashiCharacterAi = _lifetimeScope.Resolve<AtarashiCharacterAi>();

        _ = Task.Run(async () =>
        {
            if (await _characterAiApi.CheckCharacterAiToken() is false)
                return;

            OpenDialog(atarashiCharacterAi);
        });
    }
}