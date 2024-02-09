using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using GlobalLowLevelHooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Waifu.Models;
using Waifu.Views;

namespace Waifu.Data;

public class Hotkeys
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly KeyboardHook _keyboardHook = new KeyboardHook();
    private readonly ILogger<Hotkeys> _logger;

    public Hotkeys(ApplicationDbContext applicationDbContext, ILogger<Hotkeys> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;

        _keyboardHook.KeyDown += KeyboardHookOnKeyDown;
        _keyboardHook.KeyUp += KeyboardHookOnKeyUp;
    }

    private void KeyboardHookOnKeyUp(KeyboardHook.VKeys key)
    {
        if (_pressedKeys.Contains(key))
            _pressedKeys.Remove(key);

        _logger.LogInformation($"HotKeys Released: {key.ToString()}");

        foreach (var hook in _keyHookDictionary)
        {
            if (hook.Value.Contains(key) && CurrentActiveHotkeyName == hook.Key)
            {
                HotkeyUp?.Invoke(this, hook.Key);
            }
        }
    }

    private void KeyboardHookOnKeyDown(KeyboardHook.VKeys key)
    {
        _pressedKeys.Add(key);

        _logger.LogInformation($"HotKeys Pressed: {key.ToString()}");

        foreach (var hook in _keyHookDictionary)
        {
            if (hook.Value.All(x => _pressedKeys.Contains(x)))
            {
                HotkeyDown?.Invoke(this, hook.Key);
            }
        }
    }

    public event EventHandler<string> HotkeyDown;
    public event EventHandler<string> HotkeyUp;

    public string? CurrentActiveHotkeyName { get; set; }

    private List<KeyboardHook.VKeys> _pressedKeys = new();

    private Dictionary<string, IEnumerable<KeyboardHook.VKeys>> _keyHookDictionary = new();

    public async Task<IEnumerable<Hotkey>> GetAllHotkeys()
    {
        return await _applicationDbContext.Hotkeys.ToListAsync();
    }

    private bool _isKeyboardHookInstalled = false;

    public async Task HookHotkeys()
    {
        if (!_isKeyboardHookInstalled)
        {
            _isKeyboardHookInstalled = true;
            _logger.LogInformation("Keyboard hook installed");
          _keyboardHook.Install();
        }

        var hotkeys = await GetAllHotkeys();

        foreach (var hotkey in hotkeys)
        {
            CurrentActiveHotkeyName = hotkey.Name;

            _keyHookDictionary.TryAdd(hotkey.Name,
                hotkey.VirtualKeyCodes.Select(x => (KeyboardHook.VKeys)KeyInterop.VirtualKeyFromKey(x)));
        }
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