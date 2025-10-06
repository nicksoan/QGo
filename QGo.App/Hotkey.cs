using System.Runtime.InteropServices;
using System.Windows.Input;

namespace QGo;
public static class Hotkey
{
    public const int WM_HOTKEY = 0x0312;
    const uint MOD_ALT = 0x0001, MOD_CTRL = 0x0002, MOD_SHIFT = 0x0004, MOD_WIN = 0x0008;

    [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
    [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public static (uint mods, int vk) Parse(string gesture)
    {
        uint mods = 0; Key key = Key.None;
        foreach (var part in gesture.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            switch (part.ToLowerInvariant())
            {
                case "alt": mods |= MOD_ALT; break;
                case "ctrl": case "control": mods |= MOD_CTRL; break;
                case "shift": mods |= MOD_SHIFT; break;
                case "win": case "windows": mods |= MOD_WIN; break;
                default: key = (Key)Enum.Parse(typeof(Key), part, true); break;
            }
        }
        return (mods, KeyInterop.VirtualKeyFromKey(key));
    }

    public static bool TryRegister(IntPtr hwnd, int id, string gesture)
    {
        var (mods, vk) = Parse(gesture);
        return RegisterHotKey(hwnd, id, mods, vk);
    }

    public static void Unregister(IntPtr hwnd, int id) => UnregisterHotKey(hwnd, id);
}
