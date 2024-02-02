using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu.Data;
using Waifu.Views.Index;
using Waifu.Views.Shared;
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
    private readonly Settings _settings;
    private readonly Header _header;

    public MainWindow(Welcome welcome, ILogger logger, StartupCheck startupCheck, MainArea mainArea, Settings settings,
        Header header)
    {
        _welcome = welcome;
        _logger = logger;
        _startupCheck = startupCheck;
        _mainArea = mainArea;
        _settings = settings;
        _header = header;

        InitializeComponent();
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        DockPanel.SetDock(_header, Dock.Top);
        MainDock.Children.Insert(0, _header);

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

    public void SetTopView<T>(T child) where T : IPopup
    {
        if (child is not FrameworkElement)
            throw new ArgumentException("child must be a FrameworkElement!");

        child.CloseTriggered += (sender, args) =>
        {
            LayerAboveContent.Children.Remove(sender as FrameworkElement);
        };

        child.ReplaceTriggered += (sender, element) =>
        {
            if (element is IPopup popupElement)
            {
                SetTopView(popupElement); // Now 'popupElement' is treated as both FrameworkElement and IPopup
            }
        };

        LayerAboveContent.Children.Add((child as FrameworkElement)!);
    }

    private void WindowsSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // fix for windows 11 where maximized window is bigger than the screen
        this.BorderThickness = this.WindowState == WindowState.Maximized
            ? new System.Windows.Thickness(8)
            : new System.Windows.Thickness(0);
    }
}