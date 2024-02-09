using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Waifu.Models;

namespace Waifu.Data;

public class Hotkeys
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Hotkeys(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IEnumerable<Hotkey>> GetAllHotkeys()
    {
        return await _applicationDbContext.Hotkeys.ToListAsync();
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