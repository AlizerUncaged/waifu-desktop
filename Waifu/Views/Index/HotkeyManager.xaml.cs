using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Waifu.Data;
using Waifu.Models;
using Waifu.Utilities;

namespace Waifu.Views.Index;

public partial class HotkeyManager : UserControl, IPopup
{
    private readonly ILogger<HotkeyManager> _logger;
    private readonly Hotkeys _hotkeys;

    public HotkeyManager(ILogger<HotkeyManager> logger, Hotkeys hotkeys)
    {
        _logger = logger;
        _hotkeys = hotkeys;
        InitializeComponent();
    }


    public event EventHandler? CloseTriggered;

    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void CancelClicked(object sender, RoutedEventArgs e)
    {
        var mainWindow = this.GetCurrentWindow() as MainWindow;

        if (_hotkeyPrev.Any())
        {
            mainWindow.ShowMessage("Save your changes first.");
            return;
        }

        CloseTriggered?.Invoke(this, EventArgs.Empty);
    }

    private void StartGetHotkey(object sender, RoutedEventArgs e)
    {
        if (sender is not Button frameworkElement) return;

        var hotkeyName = frameworkElement.Tag.ToString();

        if (string.IsNullOrWhiteSpace(hotkeyName)) return;

        var mainWindow = this.GetCurrentWindow() as MainWindow;

        _activeTextBlock = this.FindName(hotkeyName + "Keys") as TextBlock;

        if (frameworkElement.Content.ToString() == "Save")
        {
            Keyboard.RemovePreviewKeyDownHandler(mainWindow, KeydownHandler);
            Keyboard.RemovePreviewKeyUpHandler(mainWindow, KeyupHandler);

            _hotkeyPrev.Clear();

            frameworkElement.Content = "Change";

            var keyCodes = ((List<Key>)_activeTextBlock.Tag);

            _ = Task.Run(async () =>
            {
                await _hotkeys.AddOrUpdateHotkeyAsync(new Hotkey()
                {
                    Name = hotkeyName,
                    VirtualKeyCodes = keyCodes.ToList()
                }, false);
            });
            return;
        }

        frameworkElement.Content = "Save";


        Keyboard.AddPreviewKeyDownHandler(mainWindow, KeydownHandler);
        Keyboard.AddPreviewKeyUpHandler(mainWindow, KeyupHandler);
    }

    private TextBlock? _activeTextBlock;

    private List<Key> _hotkeyPrev = new();

    private void KeyupHandler(object sender, KeyEventArgs e)
    {
        _isPressing = false;

        _logger.LogInformation($"Key released: {e.Key}");

        UpdateKeyBlock();
    }

    public void UpdateKeyBlock()
    {
        if (_activeTextBlock is not null)
            _activeTextBlock.Text = $"{string.Join("+", _hotkeyPrev.Select(x => x.ToString()))}";
    }

    private bool _isPressing = false;

    private void KeydownHandler(object sender, KeyEventArgs e)
    {
        // aaaaaaaaaaaaaaaaaa

        VoiceKeys.Tag ??= new List<Key>();

        var keys = (List<Key>)VoiceKeys.Tag;

        if (!_isPressing)
        {
            _hotkeyPrev.Clear();
            keys.Clear();
        }

        if (_hotkeyPrev.LastOrDefault() is { } lastKey && lastKey != e.Key)
        {
            _hotkeyPrev.Add(e.Key);
            keys.Add(e.Key);

            _logger.LogInformation($"Key pressed: {e.Key}");
        }

        UpdateKeyBlock();

        _isPressing = true;
    }

    private void HotkeyManagerLoaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            var hotkeys = await _hotkeys.GetAllHotkeys();

            foreach (var hotkey in hotkeys)
            {
                Dispatcher.Invoke(() =>
                {
                    if (this.FindName(hotkey.Name + "Keys") is TextBlock textBlock)
                    {
                        textBlock.Text = $"{string.Join("+", hotkey.VirtualKeyCodes.Select(x => x.ToString()))}";
                    }
                });
            }
        });
    }
}