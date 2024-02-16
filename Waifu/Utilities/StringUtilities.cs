using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using SharpHook.Native;

namespace Waifu.Utilities;

public static class StringUtilities
{
    public static string RemoveSpecialCharacters(this string str)
    {
        string result = str.Replace("~", ", ")
            .Replace("*", ", ")
            .Replace("_", ", ")
            .Replace("|", ",")
            .Replace("\n", ".");

        return result;
    }

    public static byte[] ToBytes(this string message) => Encoding.UTF8.GetBytes(message);


    public static KeyCode FromWpfKey(this Key key)
    {
        // gaming
        switch (key)
        {
            case Key.None:
                return KeyCode.VcUndefined;
            case Key.Cancel:
                return KeyCode.VcCancel;
            case Key.Tab:
                return KeyCode.VcTab;
            case Key.LineFeed:
                return KeyCode.VcLineFeed;
            case Key.Enter:
                return KeyCode.VcEnter;
            case Key.Pause:
                return KeyCode.VcPause;
            case Key.CapsLock:
                return KeyCode.VcCapsLock;
            case Key.Escape:
                return KeyCode.VcEscape;
            case Key.Space:
                return KeyCode.VcSpace;
            case Key.PageUp:
                return KeyCode.VcPageUp;
            case Key.PageDown:
                return KeyCode.VcPageDown;
            case Key.End:
                return KeyCode.VcEnd;
            case Key.Home:
                return KeyCode.VcHome;
            case Key.Left:
                return KeyCode.VcLeft;
            case Key.Up:
                return KeyCode.VcUp;
            case Key.Right:
                return KeyCode.VcRight;
            case Key.Down:
                return KeyCode.VcDown;
            case Key.Select:
                return KeyCode.VcSelect;
            case Key.Print:
                return KeyCode.VcPrint;
            case Key.Execute:
                return KeyCode.VcExecute;
            case Key.PrintScreen:
                return KeyCode.VcPrintScreen;
            case Key.Insert:
                return KeyCode.VcInsert;
            case Key.Delete:
                return KeyCode.VcDelete;
            case Key.Help:
                return KeyCode.VcHelp;
            case Key.D0:
                return KeyCode.Vc0;
            case Key.D1:
                return KeyCode.Vc1;
            case Key.D2:
                return KeyCode.Vc2;
            case Key.D3:
                return KeyCode.Vc3;
            case Key.D4:
                return KeyCode.Vc4;
            case Key.D5:
                return KeyCode.Vc5;
            case Key.D6:
                return KeyCode.Vc6;
            case Key.D7:
                return KeyCode.Vc7;
            case Key.D8:
                return KeyCode.Vc8;
            case Key.D9:
                return KeyCode.Vc9;
            case Key.A:
                return KeyCode.VcA;
            case Key.B:
                return KeyCode.VcB;
            case Key.C:
                return KeyCode.VcC;
            case Key.D:
                return KeyCode.VcD;
            case Key.E:
                return KeyCode.VcE;
            case Key.F:
                return KeyCode.VcF;
            case Key.G:
                return KeyCode.VcG;
            case Key.H:
                return KeyCode.VcH;
            case Key.I:
                return KeyCode.VcI;
            case Key.J:
                return KeyCode.VcJ;
            case Key.K:
                return KeyCode.VcK;
            case Key.L:
                return KeyCode.VcL;
            case Key.M:
                return KeyCode.VcM;
            case Key.N:
                return KeyCode.VcN;
            case Key.O:
                return KeyCode.VcO;
            case Key.P:
                return KeyCode.VcP;
            case Key.Q:
                return KeyCode.VcQ;
            case Key.R:
                return KeyCode.VcR;
            case Key.S:
                return KeyCode.VcS;
            case Key.T:
                return KeyCode.VcT;
            case Key.U:
                return KeyCode.VcU;
            case Key.V:
                return KeyCode.VcV;
            case Key.W:
                return KeyCode.VcW;
            case Key.X:
                return KeyCode.VcX;
            case Key.Y:
                return KeyCode.VcY;
            case Key.Z:
                return KeyCode.VcZ;
            case Key.LWin:
                return KeyCode.VcLeftMeta;
            case Key.RWin:
                return KeyCode.VcRightMeta;
            case Key.Apps:
                return KeyCode.VcContextMenu;
            case Key.Sleep:
                return KeyCode.VcSleep;
            case Key.NumPad0:
                return KeyCode.VcNumPad0;
            case Key.NumPad1:
                return KeyCode.VcNumPad1;
            case Key.NumPad2:
                return KeyCode.VcNumPad2;
            case Key.NumPad3:
                return KeyCode.VcNumPad3;
            case Key.NumPad4:
                return KeyCode.VcNumPad4;
            case Key.NumPad5:
                return KeyCode.VcNumPad5;
            case Key.NumPad6:
                return KeyCode.VcNumPad6;
            case Key.NumPad7:
                return KeyCode.VcNumPad7;
            case Key.NumPad8:
                return KeyCode.VcNumPad8;
            case Key.NumPad9:
                return KeyCode.VcNumPad9;
            case Key.Multiply:
                return KeyCode.VcNumPadMultiply;
            case Key.Add:
                return KeyCode.VcNumPadAdd;
            case Key.Subtract:
                return KeyCode.VcNumPadSubtract;
            case Key.Decimal:
                return KeyCode.VcNumPadDecimal;
            case Key.Divide:
                return KeyCode.VcNumPadDivide;
            case Key.F13:
                return KeyCode.VcF13;
            case Key.F14:
                return KeyCode.VcF14;
            case Key.F15:
                return KeyCode.VcF15;
            case Key.F16:
                return KeyCode.VcF16;
            case Key.F17:
                return KeyCode.VcF17;
            case Key.F18:
                return KeyCode.VcF18;
            case Key.F19:
                return KeyCode.VcF19;
            case Key.F20:
                return KeyCode.VcF20;
            case Key.F21:
                return KeyCode.VcF21;
            case Key.F22:
                return KeyCode.VcF22;
            case Key.F23:
                return KeyCode.VcF23;
            case Key.F24:
                return KeyCode.VcF24;
            case Key.NumLock:
                return KeyCode.VcNumLock;
            case Key.Scroll:
                return KeyCode.VcScrollLock;
            case Key.LeftShift:
                return KeyCode.VcLeftShift;
            case Key.RightShift:
                return KeyCode.VcRightShift;
            case Key.LeftCtrl:
                return KeyCode.VcLeftControl;
            case Key.RightCtrl:
                return KeyCode.VcRightControl;
            case Key.LeftAlt:
                return KeyCode.VcLeftAlt;
            case Key.RightAlt:
                return KeyCode.VcRightAlt;
            case Key.VolumeUp:
                return KeyCode.VcVolumeUp;
            case Key.VolumeDown:
                return KeyCode.VcVolumeDown;
            case Key.MediaNextTrack:
                return KeyCode.VcMediaNext;
            case Key.MediaPreviousTrack:
                return KeyCode.VcMediaPrevious;
            case Key.MediaStop:
                return KeyCode.VcMediaStop;
            case Key.MediaPlayPause:
                return KeyCode.VcMediaPlay;
        }

        // HOW???????????????????
        return (KeyCode)KeyInterop.VirtualKeyFromKey(key);
    }


    public static string GetSha512(this string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        using (var hash = System.Security.Cryptography.SHA512.Create())
        {
            var hashedInputBytes = hash.ComputeHash(bytes);

            // Convert to text
            // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
            var hashedInputStringBuilder = new System.Text.StringBuilder(128);
            foreach (var b in hashedInputBytes)
                hashedInputStringBuilder.Append(b.ToString("X2"));
            return hashedInputStringBuilder.ToString();
        }
    }

    public static string ToHotkeyString(this IEnumerable<Key> keys)
    {
        return
            $"{string.Join("+", keys.Select(x => x.ToString()))}";
    }

    public static void OpenAsUrl(this string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            // we wont reach this far
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}