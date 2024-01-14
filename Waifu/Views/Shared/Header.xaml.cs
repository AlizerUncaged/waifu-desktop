using System.Windows.Controls;
using System.Windows.Input;

namespace Waifu.Views.Shared;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    private void HeaderMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            OnHeaderMouseDown?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler OnHeaderMouseDown;

    private void CloseClicked(object sender, MouseButtonEventArgs e)
    {
        Environment.Exit(Environment.ExitCode);
    }

    private void MinimizeClicked(object sender, MouseButtonEventArgs e)
    {
        OnMinimizeClicked?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler OnMinimizeClicked;
}