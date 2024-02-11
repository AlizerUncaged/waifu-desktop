namespace Waifu.Data;

public class WatcherManager
{
    private readonly Hotkeys _hotkeys;
    private readonly AudioRecorder _audioRecorder;

    public WatcherManager(Hotkeys hotkeys, AudioRecorder audioRecorder)
    {
        _hotkeys = hotkeys;
        _audioRecorder = audioRecorder;
    }

    public void DisableAllWatchers()
    {
        _hotkeys.DoWatchHotkey = false;
        _audioRecorder.DoRecord = false;
    }

    public void EnableAllWatchers()
    {
        _hotkeys.DoWatchHotkey = true;
        _audioRecorder.DoRecord = true;
    }
}