using System.Windows;
using System.Windows.Controls;
using Waifu.Data;

namespace Waifu.Views.Shared.Popups;

public partial class AtarashiCharacterAi : UserControl, IPopup
{
    private readonly CharacterAiApi _characterAiApi;

    public AtarashiCharacterAi(CharacterAiApi characterAiApi)
    {
        _characterAiApi = characterAiApi;
        InitializeComponent();
    }

    public event EventHandler? CloseTriggered;
    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void SaveCharacter(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement) return;

        frameworkElement.IsEnabled = false;

        var characterId = CharacterId.Text;

        _ = Task.Run(async () =>
        {
            var characterData = await _characterAiApi.GetCharacterDataFromId(characterId);

            Dispatcher.Invoke(() =>
            {
                frameworkElement.IsEnabled = true;
            });
        });
    }
}