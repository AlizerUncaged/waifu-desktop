namespace Waifu.Data;

public class EventMaster // should call to the ui
{
    public event EventHandler<string> ErrorReceived;
    public event EventHandler<string> InfoReceived;

    public bool IsShuttingDown { get; set; } = false;

    public event EventHandler ShuttingDown;

    public void TriggerShutDown()
    {
        IsShuttingDown = true;
        
        ShuttingDown?.Invoke(this, EventArgs.Empty);
    }


    public void TriggerError(string error) => ErrorReceived?.Invoke(this, error);
    public void TriggerInfo(string info) => InfoReceived?.Invoke(this, info);
}