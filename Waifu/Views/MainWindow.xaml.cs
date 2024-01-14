using System.Windows;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu.Data;
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
    private readonly StartupCheck _startupCheck;
    private readonly MainArea _mainArea;

    public MainWindow(Welcome welcome, ILogger logger, StartupCheck startupCheck, MainArea mainArea)
    {
        _welcome = welcome;
        _logger = logger;
        _startupCheck = startupCheck;
        _mainArea = mainArea;

        InitializeComponent();
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        Header.OnHeaderMouseDown += (_, args) => this.DragMove();
        Header.OnMinimizeClicked += (_, args) => WindowState = WindowState.Minimized;

        _startupCheck.OnCheckFinishedSuccessfully += (o, args) =>
        {
            // at this point everything should be already loaded!
            Dispatcher.Invoke(() => SetView(_mainArea));
        };

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