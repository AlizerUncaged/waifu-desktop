using System.Windows;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu.Views.Index;
using ILogger = Serilog.ILogger;

namespace Waifu.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Welcome _welcome;
    private readonly ILogger _logger;

    public MainWindow(Welcome welcome, ILogger logger)
    {
        _welcome = welcome;
        _logger = logger;

        InitializeComponent();
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        Header.OnHeaderMouseDown += (_, args) => this.DragMove();
        Header.OnMinimizeClicked += (_, args) => WindowState = WindowState.Minimized;

        SetView(_welcome);

        _logger.Debug("MainWindow loaded completely");
    }

    /// <summary>
    /// Set's the current main content of the window.
    /// </summary>
    private void SetView(FrameworkElement child)
    {
        Main.Children.Clear();

        Main.Children.Add(child);
    }
}