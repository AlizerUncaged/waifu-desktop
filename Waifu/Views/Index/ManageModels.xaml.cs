using System.Windows;
using System.Windows.Controls;

namespace Waifu.Views.Index;

public partial class ManageModels : UserControl, IPopup
{
    public ManageModels()
    {
        InitializeComponent();
    }

    public event EventHandler? CloseTriggered;
    
    public event EventHandler<FrameworkElement>? ReplaceTriggered;
}