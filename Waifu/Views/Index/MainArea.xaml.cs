using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Waifu.Views.Shared.Popups;

namespace Waifu.Views.Index;

public partial class MainArea : UserControl
{
    private readonly AtarashiCharacter _atarashiCharacter;

    public MainArea(AtarashiCharacter atarashiCharacter)
    {
        _atarashiCharacter = atarashiCharacter;
        InitializeComponent();
    }

    /// <summary>
    /// Set's the current main content of the window.
    /// </summary>
    private void OpenDialog<T>(T child) where T : FrameworkElement, IPopup
    {
        DialogArea.Children.Clear();

        DialogArea.Children.Add(child);

        child.CloseTriggered += (sender, args) => { PopupDialogs.IsOpen = false; };

        PopupDialogs.IsOpen = true;
    }

    private void NewCharacter(object sender, MouseButtonEventArgs e)
    {
        OpenDialog(_atarashiCharacter);
    }
}