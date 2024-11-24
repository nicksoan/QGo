using Microsoft.Win32;
using QGo.Functions;
using QGo.Models;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Color = System.Windows.Media.Color;
using TextBox = System.Windows.Controls.TextBox;


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
        private UIService _uiService;

        public MainWindow()
        {
            string relativePath = @"Data\shortcuts.json";

            // Combine with the application's base directory
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);


            _parser = new CommandParser(fullPath);
            InitializeComponent();
            InitializeNotifyIcon();
            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;

            _uiService = new UIService(
                textBox: queryText,
                currentCharsList: currentCharsList,
                textChangedHandler: queryText_TextChanged,
                defaultColour: null,
                _previousTextLength
                );
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = false,
                Text = "QGo"
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
                    _uiService.ClearText();
                    HideWindow();
                }
                else
                {
                    _uiService.FlashError();
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
            _uiService.CheckClearReset();

            bool isDeleting = currentCharsList.Count() < _previousTextLength;

            // Check if user is deleting text,so do not trigger autocomplete
            if (isDeleting)
            {
                _uiService.SetDefaultBGColour();
                return;
            }

            // Proceed with autocomplete logic
            string autoCompleteTemp = new string(currentCharsList.ToArray());
            AutocompleteText(autoCompleteTemp);

            // Update previous text length
            _previousTextLength = currentCharsList.Count();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                HandleDelete(e, deletePreviousChar: true);
            }
            else if (e.Key == Key.Delete)
            {
                HandleDelete(e, deletePreviousChar: false);
            }
        }

        private void HandleDelete(System.Windows.Input.KeyEventArgs e, bool deletePreviousChar = true)
        {
            try
            {
                int caretIndex = queryText.CaretIndex;
                queryText.TextChanged -= queryText_TextChanged; // Temporarily disconnect event

                string currentText = new string(currentCharsList.ToArray());

                if (currentText == queryText.Text)
                {
                    _uiService.SetDefaultBGColour();
                }

                _uiService.CheckClearReset();

                _previousTextLength = queryText.Text.Length;
                if (queryText.SelectionLength > 0)
                {
                    // Remove the highlighted text
                    int selectionStart = queryText.SelectionStart;
                    queryText.Text = queryText.Text.Remove(selectionStart, queryText.SelectionLength);
                    Debug.WriteLine($"currentCharsList:'{new string(currentCharsList.ToArray())}'. queryText: '{queryText.Text}' ");
                    // Update the caret position after removing highlighted text
                    queryText.CaretIndex = selectionStart;

                    if (deletePreviousChar)
                    {
                        // Also delete the character before the original caret position
                        if (selectionStart > 0)
                        {

                            queryText.Text = queryText.Text.Remove(selectionStart - 1, 1);
                            queryText.CaretIndex = selectionStart - 1;
                            queryText.SelectionLength = 0;//currentText.Length;
                            queryText.Focus();
                            Debug.WriteLine($"currentCharsList:'{new string(currentCharsList.ToArray())}'. queryText: '{queryText.Text}' ");
                        }
                    }
                    _previousTextLength = queryText.Text.Length;

                }
                else if (caretIndex > 0)
                {
                    // No highlighted text; delete the character before the caret
                    queryText.Text = queryText.Text.Remove(caretIndex - 1, 1);
                    queryText.CaretIndex = caretIndex - 1;
                    queryText.SelectionLength = 0;//currentText.Length;
                    queryText.Focus();
                    Debug.WriteLine($"currentCharsList:'{new string(currentCharsList.ToArray())}'. queryText: '{queryText.Text}' ");
                }
                //else if (caretIndex == 0)
                //{
                //    _uiService.ClearText();
                //}

                currentCharsList.Clear();
                currentCharsList = queryText.Text.ToCharArray().ToList();
                Debug.WriteLine($"currentCharsList:'{new string(currentCharsList.ToArray())}'. queryText: '{queryText.Text}' ");

                // Prevent the default behavior
                e.Handled = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                queryText.TextChanged += queryText_TextChanged;
            }
        }

        private void AutocompleteText(string currentText)
        {
            if (string.IsNullOrEmpty(currentText))
            {
                _uiService.SetDefaultBGColour();
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
                    _uiService.SetDefaultBGColour();
                }
            }
            else
            {
                _uiService.SetDefaultBGColour();
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
                int cursorPosition = MatchFunctions.FindOverlapIndex(match, currentText);
                string remainingText = match.Substring(currentText.Length);
                queryText.Text = match;//currentText + remainingText;
                queryText.SelectionStart = cursorPosition;//currentText.Length;
                queryText.SelectionLength = match.Length - cursorPosition;//remainingText.Length;
                queryText.Focus();
                Debug.WriteLine($"currentCharsList: '{new string(currentCharsList.ToArray())}'.queryText: '{queryText.Text}'");

            }
            finally
            {
                queryText.TextChanged += queryText_TextChanged;
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