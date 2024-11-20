using Microsoft.Win32;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QGo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandParser _parser;
        private NotifyIcon _notifyIcon;
        private IntPtr _windowHandle;

        public MainWindow()
        {
            _parser = new CommandParser();
            InitializeComponent();
            InitializeNotifyIcon();
            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = false,
                Text = "SlickRunReplica"
            };
            _notifyIcon.DoubleClick += (s, e) => ShowWindow();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Execute the command when Enter key is pressed
                string command = queryText.Text;
                _parser.ExecuteCommand(command);
            }
            else if (e.Key == Key.Escape)
            {
                // Hide the window and minimize to system tray when Escape key is pressed
                HideWindow();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus on the textbox when the window loads
            queryText.Focus();
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;
            RegisterHotKey();
        }

        private void queryText_TextChanged(object sender, TextChangedEventArgs e)
        {
            string currentText = queryText.Text;
            var matches = _parser.GetMatchingShortcuts(currentText);

            // If there's a matching shortcut, autocomplete the text box
            foreach (var match in matches)
            {
                queryText.TextChanged -= queryText_TextChanged;
                queryText.Text = match;
                queryText.SelectionStart = currentText.Length;
                queryText.SelectionLength = match.Length - currentText.Length;
                queryText.TextChanged += queryText_TextChanged;
                break;
            }
        }

        private void HideWindow()
        {
            this.Hide();
            _notifyIcon.Visible = true;
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            _notifyIcon.Visible = false;
            queryText.Focus();
        }

        private void RegisterHotKey()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                HotKey.RegisterHotKey(_windowHandle, Key.Q, ModifierKeys.Alt, OnHotKeyHandler);
            }
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            ShowWindow();
        }


        private void mnuEditShortcuts_Click(object sender, RoutedEventArgs e)
        {
            var editShortcutsWindow = new EditShortcutsWindow();
            editShortcutsWindow.Show();
        }
    }
}