using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;
using Waifu.Models;
using Waifu.Utilities;
using Waifu.Views;

namespace Waifu.Data;

public class Hotkeys
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<Hotkeys> _logger;
    private static TaskPoolGlobalHook _taskPoolGlobalHook = new();

    private Dictionary<IEnumerable<KeyCode>, string> _hotkeyActions = new Dictionary<IEnumerable<KeyCode>, string>();

    public bool DoWatchHotkey { get; set; } = false;

    public Hotkeys(ApplicationDbContext applicationDbContext, ILogger<Hotkeys> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;


        _taskPoolGlobalHook.KeyPressed += KeyboardHookOnKeyDown;
        _taskPoolGlobalHook.KeyReleased += KeyboardHookOnKeyUp;
    }

    private void KeyboardHookOnKeyUp(object? sender, KeyboardHookEventArgs args)
    {
        if (!DoWatchHotkey)
            return;

        var key = args.Data.KeyCode;


        if (_pressedIndividualKeys.Contains(key))
        {
            _pressedIndividualKeys.Remove(key);

            // Check if released key is part of any hotkey combination
            foreach (var hotkeyCombination in _hotkeyActions.Keys)
            {
                if (hotkeyCombination.Contains(key) && _pressedIndividualKeys.Intersect(hotkeyCombination).Count() == 0)
                {
                    var hotkeyName = _hotkeyActions[hotkeyCombination];

                    if (!_downHotkeys.Contains(hotkeyName))
                        continue;

                    _downHotkeys.Remove(hotkeyName);

                    _logger.LogInformation($"Hotkey up {hotkeyName}");

                    HotkeyUp?.Invoke(this, hotkeyName);
                }
            }
        }
    }


    private void KeyboardHookOnKeyDown(object? sender, KeyboardHookEventArgs args)
    {
        if (!DoWatchHotkey)
            return;

        var key = args.Data.KeyCode;


        _pressedIndividualKeys.Add(key);

        // Check if any hotkey combination is pressed
        foreach (var hotkeyCombination in _hotkeyActions.Keys)
        {
            if (hotkeyCombination.All(x => _pressedIndividualKeys.Contains(x)))
            {
                var hotkeyName = _hotkeyActions[hotkeyCombination];

                if (_downHotkeys.Contains(hotkeyName))
                    continue;

                _downHotkeys.Add(hotkeyName);

                _logger.LogInformation($"Hotkey down {hotkeyName}");

                HotkeyDown?.Invoke(this, _hotkeyActions[hotkeyCombination]);
            }
        }
    }

    private readonly HashSet<KeyCode> _pressedIndividualKeys = new();
    private readonly HashSet<string> _downHotkeys = new();

    public event EventHandler<string>? HotkeyDown;
    public event EventHandler<string>? HotkeyUp;


    public async Task<IEnumerable<Hotkey>> GetAllHotkeys()
    {
        var cacheHotkeyObj = new Dictionary<IEnumerable<KeyCode>, string>();

        var hotkeys = await _applicationDbContext.Hotkeys.ToListAsync(); // auto cache it

        foreach (var hotkey in hotkeys)
        {
            var optKeys = hotkey.VirtualKeyCodes.Select(x => x.FromWpfKey());

            _logger.LogInformation(
                $"Registering hotkey {hotkey.Name}: {string.Join(", ", optKeys.Select(x => $"{x} or {(int)x}"))}");

            cacheHotkeyObj.Add(optKeys,
                hotkey.Name);
        }

        _hotkeyActions = cacheHotkeyObj;

        return hotkeys;
    }

    private bool _isKeyboardHookInstalled = false;

    public async Task HookHotkeys()
    {
        if (!_isKeyboardHookInstalled)
        {
            _isKeyboardHookInstalled = true;

            _logger.LogInformation("Keyboard hook installed");
        }

        // breakpoints are laggy if the hook is on

        _ = _taskPoolGlobalHook.RunAsync();
    }


    public event EventHandler<string> HotkeyManageMessage;

    public async Task<Hotkey> AddOrUpdateHotkeyAsync(Hotkey hotkey, bool dontNotify = true)
    {
        var existingHotkey = await _applicationDbContext.Hotkeys.FirstOrDefaultAsync(x => x.Name == hotkey.Name);

        if (existingHotkey is { })
        {
            _applicationDbContext.Hotkeys.Remove(existingHotkey);

            await _applicationDbContext.SaveChangesAsync();
        }

        _applicationDbContext.Hotkeys.Add(hotkey);

        await _applicationDbContext.SaveChangesAsync();

        if (!dontNotify)
            HotkeyManageMessage?.Invoke(this, $"Hotkey for {hotkey.Name} added!");

        await GetAllHotkeys(); // recache hotkeys
        return hotkey;
    }

    public async Task<Hotkey> GetOrAddHotkeyAsync(string hotkeyName, params Key[] defaultHotkey)
    {
        var existingHotkey = await _applicationDbContext.Hotkeys.FirstOrDefaultAsync(x => x.Name == hotkeyName);

        if (existingHotkey is { })
            return existingHotkey;

        var newHotkey = new Hotkey()
        {
            Name = hotkeyName, VirtualKeyCodes = defaultHotkey.ToList()
        };

        _applicationDbContext.Hotkeys.Add(newHotkey);

        await _applicationDbContext.SaveChangesAsync();

        return newHotkey;
    }
}