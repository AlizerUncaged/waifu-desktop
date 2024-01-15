using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Waifu.Utilities;

namespace Waifu.Views.Shared;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    private void HeaderMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e is { ChangedButton: MouseButton.Left, ButtonState: MouseButtonState.Pressed })
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

    public event EventHandler OnMaximizeClicked;
    public event EventHandler OnSettingsClicked;

    private void MaximizeClicked(object sender, MouseButtonEventArgs e)
    {
        OnMaximizeClicked?.Invoke(this, EventArgs.Empty);
    }

    private void GithubClicked(object sender, MouseButtonEventArgs e) =>
        "https://github.com/AlizerUncaged/waifu-desktop".OpenAsUrl();

    private void SettingsClicked(object sender, MouseButtonEventArgs e)
    {
        OnSettingsClicked?.Invoke(this, EventArgs.Empty);
    }
}