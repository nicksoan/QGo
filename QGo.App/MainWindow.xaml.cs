using QGo.App;
using QGo.App.Models;
using QGo.App.Views;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace QGo.App;
public partial class MainWindow : Window
{
    private readonly int _hotkeyId = 0xBEEF;
    private MainViewModel Vm => (MainViewModel)DataContext;

    private readonly string _storePath =
       Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QGo", "Data", "window.json");
    
    private bool _loaded;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Loaded += (_, __) =>
        {
            //Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
            //Top = SystemParameters.WorkArea.Top + 60;

            RestorePlacement();
            _loaded = true;

            var handle = new WindowInteropHelper(this).Handle;
            Hotkey.TryRegister(handle, _hotkeyId, Vm.Settings.HotkeyGesture);

            var src = HwndSource.FromHwnd(handle);
            src.AddHook(WndProc);

            Hide();
        };

        LocationChanged += (_, __) => SavePlacement();
        SizeChanged += (_, __) => SavePlacement();
        StateChanged += (_, __) => SavePlacement();

        // Key handling for navigation and selection
        PreviewKeyDown += (s, e) =>
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Hide();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    Vm.LaunchSelected();
                    Hide();
                    e.Handled = true;
                    break;

                case Key.Down:
                    if (Vm.FilteredItems.MoveCurrentToNext())
                        Vm.Selected = Vm.FilteredItems.CurrentItem as Shortcut;
                    e.Handled = true;
                    break;

                case Key.Up:
                    if (Vm.FilteredItems.MoveCurrentToPrevious())
                        Vm.Selected = Vm.FilteredItems.CurrentItem as Shortcut;
                    e.Handled = true;
                    break;
            }
        };

        Deactivated += (_, __) => Hide();
    }

    // Toggle visibility when hotkey pressed
    private void ToggleShow()
    {
        if (IsVisible)
        {
            Hide();
            return;
        }
        Vm.SearchText = string.Empty;
        Show();
        Activate();
        Input.Focus();
        Input.SelectAll();
    }

    // Intercept Windows hotkey message
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Hotkey.WM_HOTKEY && wParam.ToInt32() == _hotkeyId)
        {
            ToggleShow();
            handled = true;
        }
        return IntPtr.Zero;
    }

    // Re-register when hotkey changed in Settings
    private void RebindHotkey()
    {
        var handle = new WindowInteropHelper(this).Handle;
        Hotkey.Unregister(handle, _hotkeyId);
        Hotkey.TryRegister(handle, _hotkeyId, Vm.Settings.HotkeyGesture);
    }

    // Context menu: open Settings dialog
    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SettingsWindow { DataContext = new SettingsViewModel(Vm.Settings) };
        if (dlg.ShowDialog() == true)
        {
            Vm.ApplyTheme();
            Storage.SaveSettings(Vm.Settings);
            RebindHotkey();
        }
    }

    // Context menu: open Manage Shortcuts dialog
    private void ManageShortcuts_Click(object sender, RoutedEventArgs e)
    {
        var vm = new ManageShortcutsViewModel(Vm.All);
        var dlg = new ManageShortcutsWindow { DataContext = vm };
        if (dlg.ShowDialog() == true)
        {
            Vm.ReloadFrom(vm.Items);
            Storage.SaveLinks(vm.Items);
        }
    }
    private void About_Click(object sender, RoutedEventArgs e)
    {
        new AboutWindow { Owner = this }.ShowDialog();
    }

    // Context menu: exit program
    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        Hotkey.Unregister(handle, _hotkeyId);
        SavePlacement();
        base.OnClosed(e);
    }

    private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        MessageBox.Show(e.ErrorException?.Message ?? "Image load failed");
    }
    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void SavePlacement()
    {
        if (!_loaded || WindowState == WindowState.Minimized) return;
        try
        {

            var dir = Path.GetDirectoryName(_storePath)!;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var hwnd = new WindowInteropHelper(this).Handle;
            var data = new WindowPlacement
            {
                Left = Left,
                Top = Top,
                Width = Width,
                Height = Height,
                IsMaximized = WindowState == WindowState.Maximized,
                MonitorDeviceName = "Unknown"
            };
            File.WriteAllText(_storePath, JsonSerializer.Serialize(data));
        }
        catch { /* ignore */ }
    }

    private void RestorePlacement()
    {
        try
        {
            if (!File.Exists(_storePath)) return;
            var data = JsonSerializer.Deserialize<WindowPlacement>(File.ReadAllText(_storePath));
            if (data == null) return;

            // Apply saved position and size
            Left = data.Left;
            Top = data.Top;
            Width = data.Width;
            Height = data.Height;

            // Ensure window is visible on current desktop work area
            var workArea = SystemParameters.WorkArea;

            if (Left + Width < workArea.Left + 50 ||
                Top + Height < workArea.Top + 50 ||
                Left > workArea.Right - 50 ||
                Top > workArea.Bottom - 50)
            {
                Left = workArea.Left + (workArea.Width - Width) / 2;
                Top = workArea.Top + (workArea.Height - Height) / 2;
            }

            if (data.IsMaximized)
                WindowState = WindowState.Maximized;
        }
        catch
        {
            // ignore
        }
    }
}
