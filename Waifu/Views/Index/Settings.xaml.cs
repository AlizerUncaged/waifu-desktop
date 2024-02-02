using System.Windows;
using System.Windows.Controls;

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
}