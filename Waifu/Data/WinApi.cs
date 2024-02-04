using System.Runtime.InteropServices;

namespace Waifu.Data;

public static class WinApi
{
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
        DWMWINDOWATTRIBUTE attribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
        uint cbAttribute);

    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    public static void AttemptRoundedCorners(IntPtr hWnd)
    {
        try
        {
            var attribute = WinApi.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = WinApi.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

            WinApi.DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
        }
        catch
        {
            // we're not on windows 11!
        }
    }
}