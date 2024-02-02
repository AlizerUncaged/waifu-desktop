using System.Windows;
using System.Windows.Controls;

namespace Waifu.Utilities;

public static class UserControlUtilities
{
    public static Window? GetCurrentWindow(this UserControl control) =>
        Window.GetWindow(control);

    public static T? GetParentType<T>(this UserControl control) where T : class
    {
        DependencyObject ucParent = control;

        while (!(ucParent is T))
        {
            ucParent = LogicalTreeHelper.GetParent(ucParent);
        }

        return ucParent as T;
    }
}