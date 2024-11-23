using Microsoft.Win32;
using QGo.Models;
using System.Diagnostics;
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
        private int _previousTextLength = 0;
        List<char> currentCharsList = new List<char>();
        private SolidColorBrush defaultColour = new SolidColorBrush(Colors.White);
        private SolidColorBrush foundColour = new SolidColorBrush(Colors.LightGreen);

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
                Debug.WriteLine("Enter pressed!");
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
            else if ((e.Key >= Key.A && e.Key <= Key.Z) || // Letters A-Z
                    (e.Key >= Key.D0 && e.Key <= Key.D9) || // Numbers 0-9 (main keyboard)
                    (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                AddKeyToCurrentCharArray(e);
            }
            else if (e.Key == Key.Escape)
            {
                // Hide the window and minimize to system tray when Escape key is pressed
                HideWindow();
            }
        }

        private void AddKeyToCurrentCharArray(System.Windows.Input.KeyEventArgs e)
        {
            string keyString = e.Key.ToString().ToLower();
            char newChar;

            // Convert numeric keys to their actual characters
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                newChar = (char)('0' + (e.Key - Key.D0)); // Convert Key.D0 to '0', Key.D1 to '1', etc.
            }
            else
            {
                // Convert letters
                newChar = keyString[0]; // Take the first character of the Key's name
            }

            string currentText = new string(currentCharsList.ToArray());
            currentCharsList.Add(newChar);
            Debug.WriteLine($"currentCharslist:'{new string(currentText)}'({currentCharsList.Count()}) Adding: '{newChar}'. = '{new string(currentCharsList.ToArray())}' Textbox Text: {queryText.Text}");


        }

        private void queryText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckClearReset();

            bool isDeleting = currentCharsList.Count() < _previousTextLength;

            // Check if user is deleting text,so do not trigger autocomplete
            if (isDeleting)
            {
                queryText.Background = defaultColour;
                return;
            }

            // Proceed with autocomplete logic
            string autoCompleteTemp = new string(currentCharsList.ToArray());
            AutocompleteText(autoCompleteTemp);

            // Update previous text length
            _previousTextLength = currentCharsList.Count();
        }

        private void AutocompleteText(string currentText)
        {
            if (string.IsNullOrEmpty(currentText))
            {
                queryText.Background = defaultColour;
                return;
            }

            var matches = _parser.GetMatchingShortcuts(currentText);

            if (matches.Any())
            {
                var match = matches.First();

                if (match.StartsWith(currentText, StringComparison.OrdinalIgnoreCase))
                {
                    ApplyFoundQuery(currentText, match);
                }
                else if (currentText.Length > 3 && match.Contains(currentText, StringComparison.OrdinalIgnoreCase))
                {
                    ApplyFoundQuery(currentText, match);
                }
                else
                {
                    queryText.Background = defaultColour;
                }
            }
            else
            {
                queryText.Background = defaultColour;
            }
        }

        private void ApplyFoundQuery(string currentText, string match)
        {
            // Temporarily detach event handler
            queryText.TextChanged -= queryText_TextChanged;

            try
            {
                queryText.Background = foundColour;
                int currentCursorPosition = queryText.CaretIndex;
                int cursorPosition = FindOverlapIndex(match, currentText);
                string remainingText = match.Substring(currentText.Length);
                queryText.Text = match;//currentText + remainingText;
                queryText.SelectionStart = cursorPosition;//currentText.Length;
                queryText.SelectionLength = match.Length - cursorPosition;//remainingText.Length;
                queryText.Focus();
            }
            finally
            {
                queryText.TextChanged += queryText_TextChanged;
            }
        }

        private int FindOverlapIndex(string match, string enteredText)
        {
            // Find where the entered text aligns with the match
            int index = match.IndexOf(enteredText, StringComparison.InvariantCultureIgnoreCase);
            if (index >= 0)
            {
                return index + enteredText.Length; // End of the overlap
            }

            // Fallback: If no clear overlap, place the cursor at the end of the match
            return match.Length;
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

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                Console.WriteLine("Backspace key was pressed.");
                try
                {
                    CheckClearReset();

                    _previousTextLength = currentCharsList.Count;

                    if (currentCharsList.Count() > 0)
                    {
                        string currentText = new string(currentCharsList.ToArray());
                        try
                        {
                            Debug.WriteLine($"currentCharslist:'{currentText}'({currentCharsList.Count()}) Removing: '{currentCharsList[currentCharsList.Count() - 1]}' = '{new string(currentCharsList.ToArray())}'. Textbox Text: {queryText.Text}");
                            currentCharsList.RemoveAt(currentCharsList.Count() - 1);
                        }
                        catch (Exception)
                        {
                            currentCharsList.Clear();
                            _previousTextLength = 0;
                        }

                    }
                    else
                    {
                        Debug.WriteLine("No chars left");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ex:{ex.Message}. currentCharsList: {currentCharsList.Count()}");
                }
            }
        }

        private void CheckClearReset()
        {
            if (queryText.Text.Length == 0)
            {
                currentCharsList.Clear();
                _previousTextLength = 0;
            }
        }
    }
}