namespace Waifu.Data;

public class EventMaster // should call to the ui
{
    public event EventHandler<string> ErrorReceived;
    public event EventHandler<string> InfoReceived;

    public void TriggerError(string error) => ErrorReceived?.Invoke(this, error);
    public void TriggerInfo(string info) => InfoReceived?.Invoke(this, info);
}