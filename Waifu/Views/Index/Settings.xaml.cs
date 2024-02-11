using System.Windows;
using System.Windows.Forms;
using Humanizer;
using Waifu.Data;
using Waifu.Utilities;
using UserControl = System.Windows.Controls.UserControl;

namespace Waifu.Views.Index;

public partial class Settings : UserControl, IPopup
{
    private readonly ModelManager _manageModels;
    private readonly HotkeyManager _hotkeyManager;
    private readonly WhisperManager _whisperManager;

    public Settings(ModelManager manageModels, HotkeyManager hotkeyManager, WhisperManager whisperManager)
    {
        _manageModels = manageModels;
        _hotkeyManager = hotkeyManager;
        _whisperManager = whisperManager;
        InitializeComponent();
    }


    public event EventHandler? CloseTriggered;

    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void CancelClicked(object sender, RoutedEventArgs e)
    {
        CloseTriggered?.Invoke(this, EventArgs.Empty);
    }

    private void ManageModelsClicked(object sender, RoutedEventArgs e)
    {
        ReplaceTriggered?.Invoke(this, _manageModels);
    }

    private void ManageAudioClicked(object sender, RoutedEventArgs e)
    {
        if (this.GetCurrentWindow() is MainWindow mainWindow)
            mainWindow.ShowMessage("Coming soon...");
    }

    private void ManagePersonasClicked(object sender, RoutedEventArgs e)
    {
        if (this.GetCurrentWindow() is MainWindow mainWindow)
            mainWindow.ShowMessage("Coming soon...");
    }

    private void ManageHotkeysClicked(object sender, RoutedEventArgs e)
    {
        ReplaceTriggered?.Invoke(this, _hotkeyManager);
    }
}