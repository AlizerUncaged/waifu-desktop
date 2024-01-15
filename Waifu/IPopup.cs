using System.Windows;

namespace Waifu;

public interface IPopup
{
    public event EventHandler CloseTriggered;
    public event EventHandler<FrameworkElement> ReplaceTriggered;
}