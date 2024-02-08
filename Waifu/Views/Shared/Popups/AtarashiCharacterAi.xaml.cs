using System.Windows;
using System.Windows.Controls;

namespace Waifu.Views.Shared.Popups;

public partial class AtarashiCharacterAi : UserControl, IPopup
{
    public AtarashiCharacterAi()
    {
        InitializeComponent();
    }

    public event EventHandler? CloseTriggered;
    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void SaveCharacter(object sender, RoutedEventArgs e)
    {
        
    }
}