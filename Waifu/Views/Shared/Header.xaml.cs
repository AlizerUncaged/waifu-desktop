using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Waifu.Utilities;
using Waifu.Views.Index;

namespace Waifu.Views.Shared;

public partial class Header : UserControl
{
    private readonly Settings _settings;

    public Header(Settings settings)
    {
        _settings = settings;
        InitializeComponent();
    }

    private void HeaderMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e is { ChangedButton: MouseButton.Left, ButtonState: MouseButtonState.Pressed })
            this.GetCurrentWindow()?.DragMove();
    }

    private void CloseClicked(object sender, MouseButtonEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MinimizeClicked(object sender, MouseButtonEventArgs e)
    {
        if (this.GetCurrentWindow() is MainWindow mainWindow)
            mainWindow.WindowState = mainWindow.WindowState == WindowState.Minimized
                ? WindowState.Normal
                : WindowState.Minimized;
    }


    private void MaximizeClicked(object sender, MouseButtonEventArgs e)
    {
        if (this.GetCurrentWindow() is MainWindow mainWindow)
            mainWindow.WindowState = WindowState.Maximized;
    }

    private void GithubClicked(object sender, MouseButtonEventArgs e) =>
        "https://github.com/AlizerUncaged/waifu-desktop".OpenAsUrl();

    private void SettingsClicked(object sender, MouseButtonEventArgs e)
    {
        if (this.GetCurrentWindow() is MainWindow mainWindow)
            mainWindow.SetTopView(_settings);
    }
}