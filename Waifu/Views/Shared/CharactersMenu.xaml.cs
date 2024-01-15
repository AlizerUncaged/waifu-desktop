using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Waifu.Data;
using Waifu.Models;

namespace Waifu.Views.Shared;

/// <summary>
/// Creates a scrollable list of characters.
/// </summary>
public partial class CharactersMenu : UserControl
{
    private readonly Characters _characters;

    // has to be public to be able to be accessed by the xaml frontend, so all operations of this collection
    // outside this class has to be done directly to this class lmao
    public ObservableCollection<RoleplayCharacter> RoleplayCharacters { get; set; }

    public CharactersMenu(Characters characters)
    {
        _characters = characters;

        RoleplayCharacters =
            new ObservableCollection<RoleplayCharacter>(Enumerable.Empty<RoleplayCharacter>());

        InitializeComponent();
    }

    private void CharactersMenuLoaded(object sender, RoutedEventArgs e)
    {
        _characters.OnCharacterAdded += (o, character) =>
        {
            Dispatcher.Invoke(() => RoleplayCharacters.Add(character));
        };

        _characters.OnCharacterRemoved += (o, character) =>
        {
            Dispatcher.Invoke(() => RoleplayCharacters.Remove(character));
        };
    }
}