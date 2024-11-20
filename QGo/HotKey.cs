using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

public class HotKey
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint VK_Q = 0x51;

    private static int _hotKeyId = 0;
    private readonly IntPtr _windowHandle;
    private readonly Action<HotKey> _action;

    public HotKey(Key key, ModifierKeys modifierKeys, Action<HotKey> action, IntPtr windowHandle)
    {
        _action = action;
        _windowHandle = windowHandle;
        _hotKeyId = _hotKeyId + 1;

        RegisterHotKey(_windowHandle, _hotKeyId, (uint)(MOD_ALT | MOD_CONTROL), (uint)KeyInterop.VirtualKeyFromKey(key));
        ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;
    }

    public static void RegisterHotKey(IntPtr hWnd, Key key, ModifierKeys modifiers, Action<HotKey> action)
    {
        var hotKey = new HotKey(key, modifiers, action, hWnd);
    }

    private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
    {
        if (msg.message != 0x0312) return;

        if ((int)msg.wParam == _hotKeyId)
        {
            _action?.Invoke(this);
            handled = true;
        }
    }

    ~HotKey()
    {
        UnregisterHotKey(_windowHandle, _hotKeyId);
    }
}