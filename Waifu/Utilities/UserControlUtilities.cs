using System.Windows;
using System.Windows.Controls;

namespace Waifu.Utilities;

public static class UserControlUtilities
{
    public static Window? GetCurrentWindow(this UserControl control) =>
        Window.GetWindow(control);
}