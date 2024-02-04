using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Waifu.Views.Index;

public partial class Settings : UserControl, IPopup
{
    private readonly ModelManager _manageModels;

    public Settings(ModelManager manageModels)
    {
        _manageModels = manageModels;
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
        MessageBox.Show("Coming soon...", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
        // todo
    }

    private void ManagePersonasClicked(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Coming soon...", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
        // todo
    }
}