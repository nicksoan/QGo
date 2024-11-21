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
using Color = System.Windows.Media.Color;


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
        private bool _isUserEditing = true; // Flag to track user edits
        private int _previousTextLength = 0;

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
                var qSearchResult = _parser.ExecuteCommand(command);
                if (qSearchResult.Success)
                {
                    HideWindow();
                }
                else
                {
                    queryText.Text = qSearchResult.Message;
                    queryText.SelectAll();
                }

            }
            else if (e.Key == Key.Escape)
            {
                // Hide the window and minimize to system tray when Escape key is pressed
                HideWindow();
            }
        }

        private void queryText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isUserEditing)
            {
                // Update previous text length and return
                _previousTextLength = queryText.Text.Length;
                return;
            }

            string currentText = queryText.Text;

            // Determine if the user is deleting text
            bool isDeleting = currentText.Length < _previousTextLength;

            if (isDeleting)
            {
                // User is deleting text, do not autocomplete
                queryText.Background = new SolidColorBrush(Colors.White);
                _previousTextLength = currentText.Length;
                return;
            }

            // Proceed with autocomplete logic
            AutocompleteText(currentText);

            // Update previous text length
            _previousTextLength = queryText.Text.Length;
        }

        private void AutocompleteText(string currentText)
        {
            if (string.IsNullOrEmpty(currentText))
            {
                queryText.Background = new SolidColorBrush(Colors.AntiqueWhite);
                return;
            }

            var matches = _parser.GetMatchingShortcuts(currentText);

            if (matches.Any())
            {
                var match = matches.First();

                if (match.StartsWith(currentText, StringComparison.OrdinalIgnoreCase) && match != currentText)
                {
                    // Temporarily detach event handler
                    queryText.TextChanged -= queryText_TextChanged;
                    _isUserEditing = false;

                    try
                    {
                        queryText.Background = new SolidColorBrush(Colors.ForestGreen);

                        // Append remaining text without selecting
                        string remainingText = match.Substring(currentText.Length);
                        queryText.Text = currentText + remainingText;
                        queryText.SelectionLength = remainingText.Length;
                        queryText.SelectionStart = currentText.Length;
                        //queryText.SelectionStart = queryText.Text.Length;
                    }
                    finally
                    {
                        _isUserEditing = true;
                        queryText.TextChanged += queryText_TextChanged;
                    }
                }
                else
                {
                    queryText.Background = new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                queryText.Background = new SolidColorBrush(Colors.DarkRed);
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