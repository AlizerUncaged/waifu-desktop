using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu.Controllers;
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
    private readonly ILogger<MainWindow> _logger;
    private readonly StartupCheck _startupCheck;
    private readonly MainArea _mainArea;
    private readonly Waifu.Data.Settings _settings;
    
    public Waifu.Data.Settings Settings => _settings;
    
    private readonly Header _header;
    private readonly CharacterAiApi _characterAiApi;
    private readonly ChatAreaController _chatAreaController;
    private readonly Hotkeys _hotkeys;
    private readonly AudioRecorder _audioRecorder;
    private readonly WhisperManager _whisperManager;
    private readonly WatcherManager _watcherManager;
    private readonly ElevenlabsVoiceGenerator _elevenlabsVoiceGenerator;

    public ElevenlabsVoiceGenerator ElevenlabsVoiceGenerator => _elevenlabsVoiceGenerator;

    public SnackbarMessageQueue SnackbarMessageQueue { get; } = new();

    public MainWindow(Welcome welcome,
        ILogger<MainWindow> logger,
        StartupCheck startupCheck,
        MainArea mainArea,
        Waifu.Data.Settings settings,
        Header header,
        CharacterAiApi characterAiApi,
        ChatAreaController chatAreaController,
        Hotkeys hotkeys,
        AudioRecorder audioRecorder,
        WhisperManager whisperManager, 
        WatcherManager watcherManager,
        ElevenlabsVoiceGenerator elevenlabsVoiceGenerator)
    {
        _welcome = welcome;
        _logger = logger;
        _startupCheck = startupCheck;
        _mainArea = mainArea;
        _settings = settings;
        _header = header;
        _characterAiApi = characterAiApi;
        _chatAreaController = chatAreaController;
        _hotkeys = hotkeys;
        _audioRecorder = audioRecorder;
        _whisperManager = whisperManager;
        _watcherManager = watcherManager;
        _elevenlabsVoiceGenerator = elevenlabsVoiceGenerator;

        InitializeComponent();

        if (GetWindow(this) is { } realWindow)
            WinApi.AttemptRoundedCorners(new WindowInteropHelper(realWindow).EnsureHandle());
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        Panel.SetZIndex(_header, 29);
        DockPanel.SetDock(_header, Dock.Top);
        MainDock.Children.Insert(0, _header);

        _startupCheck.OnCheckFinishedSuccessfully += (o, args) =>
        {
            // at this point everything should be already loaded!
            Dispatcher.Invoke(() => SetView(_mainArea));
        };

        _characterAiApi.ApiNotificationMessage += (o, s) => { ShowMessage(s.Trim()); };
        _chatAreaController.ChatAreaMessage += (o, s) => { ShowMessage(s.Trim()); };
        _hotkeys.HotkeyManageMessage += (o, s) => { ShowMessage(s.Trim()); };

        SetView(_welcome);

        _logger.LogDebug("MainWindow loaded completely");
        
        _watcherManager.EnableAllWatchers();
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
        if (LayerAboveContent.Children.Contains(child as FrameworkElement))
            return;

        child.CloseTriggered += (sender, args) =>
        {
            LayerAboveContent.Children.Remove(sender as FrameworkElement);

            if (LayerAboveContent.Children.Count <= 0)
                _watcherManager.EnableAllWatchers();
        };

        child.ReplaceTriggered += (sender, element) =>
        {
            if (element is IPopup popupElement)
            {
                SetTopView(popupElement); // Now 'popupElement' is treated as both FrameworkElement and IPopup
            }
        };

        LayerAboveContent.Children.Add((child as FrameworkElement)!);
        _watcherManager.DisableAllWatchers(); // we have a popup, dont do any interaction
    }

    private void WindowsSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // fix for windows 11 where maximized window is bigger than the screen
        this.BorderThickness = this.WindowState == WindowState.Maximized
            ? new System.Windows.Thickness(8)
            : new System.Windows.Thickness(0);
    }

    public void ShowMessage(string message)
    {
        Dispatcher.Invoke(() =>
        {
            SnackbarMessageQueue.Clear();
            SnackbarMessageQueue.Enqueue(message);
        });
    }

    private void MessageClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is Snackbar snackbar)
        {
            snackbar.MessageQueue?.Clear();
        }
    }
}