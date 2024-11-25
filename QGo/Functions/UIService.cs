using QGo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Media;
using System.Windows.Threading;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using TextBox = System.Windows.Controls.TextBox;

namespace QGo.Functions
{
    public class UIService
    {
        private readonly Window _window;
        private readonly TextBox _textBox;
        private readonly List<char> _currentCharsList;
        private readonly TextChangedEventHandler _textChangedHandler;
        private readonly UserSettings _userSettings;
        private SolidColorBrush _defaultColour = new SolidColorBrush(Colors.White);
        private SolidColorBrush _defaultFontColour = new SolidColorBrush(Colors.Black);
        private SolidColorBrush _foundColour = new SolidColorBrush(Colors.ForestGreen);
        private SolidColorBrush _foundFontColour = new SolidColorBrush(Colors.Black);
        private int _textBoxFontSize = 12;
        private SolidColorBrush _errorColour = new SolidColorBrush(Colors.LightPink);
        private int _previousTextLength = 0;
        private readonly CommandParser _parser;

        public UIService(Window window, TextBox textBox, List<char> currentCharsList, TextChangedEventHandler textChangedHandler, UserSettings userSettings, int previousTextLength, CommandParser commandParser)
        {
            _parser = commandParser;

            _window = window;
            _textBox = textBox;
            _userSettings = userSettings;
            _currentCharsList = currentCharsList;
            _textChangedHandler = textChangedHandler;

            ApplySettings(userSettings);
            //if (userSettings.WindowColour != null)
            //{
            //    if (userSettings.WindowColour != null)
            //    {
            //        _defaultColour = new SolidColorBrush((Color)ColorConverter.ConvertFromString(userSettings.WindowColour));
            //    }
            //}
            SetTextBoxProps();
            _previousTextLength = previousTextLength;
        }

        public void UpdateUserSettings(UserSettings userSettings)
        {
            _userSettings.WindowColour = userSettings.WindowColour;
            _userSettings.FontColour = userSettings.FontColour;
            _userSettings.FontSize = userSettings.FontSize;
            _userSettings.WindowPositionX = userSettings.WindowPositionX;
            _userSettings.WindowPositionY = userSettings.WindowPositionY;

            ApplySettings(userSettings);
        }

        public void ClearText()
        {
            _textBox.TextChanged -= _textChangedHandler;

            _textBox.Clear();
            _currentCharsList.Clear();

            _textBox.TextChanged += _textChangedHandler;

            SetTextBoxDefault();
        }

        public void SetTextBoxProps()
        {
            _textBox.Background = _defaultColour;
            _textBox.Foreground = _defaultFontColour;
            _textBox.FontSize = _userSettings.FontSize;
        }

        public void SetTextBoxDefault()
        {
            _textBox.Background = _defaultColour;
            _textBox.Foreground = _defaultFontColour;
        }

        public void SetTextBoxFound()
        {
            _textBox.Background = _foundColour;
            _textBox.Foreground = _foundFontColour;
        }

        public void CheckClearReset()
        {
            if (_textBox.Text.Length == 0)
            {
                _currentCharsList.Clear();
                _previousTextLength = 0;
                SetTextBoxDefault();
            }
        }

        public void FlashError(int durationMs = 500, int intervalMs = 150)
        {
            var timer = new DispatcherTimer();
            var elapsed = 0;
            var isErrorColor = false;

            // Set the timer interval
            timer.Interval = TimeSpan.FromMilliseconds(intervalMs);

            // Event to handle the color flashing
            timer.Tick += (sender, e) =>
            {
                if (elapsed >= durationMs)
                {
                    // Stop flashing and reset to the default color
                    SetTextBoxDefault();
                    timer.Stop();
                }
                else
                {
                    // Toggle between the error color and default color
                    _textBox.Background = isErrorColor ? _defaultColour : _errorColour;
                    isErrorColor = !isErrorColor;
                    elapsed += intervalMs;
                }
            };

            // Start the timer
            timer.Start();
        }

        public void AutocompleteText(string currentText)
        {
            if (string.IsNullOrEmpty(currentText))
            {
                SetTextBoxDefault();
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
                else if (currentText.Length > 2 && match.Contains(currentText, StringComparison.OrdinalIgnoreCase))
                {
                    ApplyFoundQuery(currentText, match);
                }
                else
                {
                    SetTextBoxDefault();
                }
            }
            else
            {
                SetTextBoxDefault();
            }
        }

        private void ApplyFoundQuery(string currentText, string match)
        {
            // Temporarily detach event handler
            _textBox.TextChanged -= _textChangedHandler;

            try
            {
                SetTextBoxFound();
                int currentCursorPosition = _textBox.CaretIndex;
                int cursorPosition = MatchFunctions.FindOverlapIndex(match, currentText);
                string remainingText = match.Substring(currentText.Length);
                _textBox.Text = match;//currentText + remainingText;
                _textBox.SelectionStart = cursorPosition;//currentText.Length;
                _textBox.SelectionLength = match.Length - cursorPosition;//remainingText.Length;
                _textBox.Focus();
                Debug.WriteLine($"currentCharsList: '{new string(_currentCharsList.ToArray())}'.queryText: '{_textBox.Text}'");

            }
            finally
            {
                _textBox.TextChanged += _textChangedHandler;
            }
        }

        public void ApplySettings(UserSettings userSettings)
        {
            try
            {
                var converter = new BrushConverter();
                if (converter.IsValid(userSettings.WindowColour))
                {
                    _defaultColour = (SolidColorBrush)converter.ConvertFromString(userSettings.WindowColour);
                    _defaultFontColour = (SolidColorBrush)converter.ConvertFromString(userSettings.FontColour);

                    _foundColour = (SolidColorBrush)converter.ConvertFromString(userSettings.FoundColour);
                    _foundFontColour = (SolidColorBrush)converter.ConvertFromString(userSettings.FontColourFound);

                    _textBoxFontSize = (int)userSettings.FontSize;

                    SetTextBoxProps();
                }

            }
            catch
            {
                // Handle invalid color
                _textBox.Background = _defaultColour;
            }

            // Apply font size
            _textBox.FontSize = userSettings.FontSize;

            // Apply window position
            _window.Left = userSettings.WindowPositionX;
            _window.Top = userSettings.WindowPositionY;
        }

        public void RestoreWindowPosition()
        {
            // Check if the saved position is within the bounds of any screen
            bool isValidPosition = false;
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                var screenBounds = screen.Bounds;
                if (screenBounds.Contains((int)_userSettings.WindowPositionX, (int)_userSettings.WindowPositionY))
                {
                    isValidPosition = true;
                    break;
                }
            }

            if (isValidPosition)
            {
                _window.Left = _userSettings.WindowPositionX;
                _window.Top = _userSettings.WindowPositionY;
                _window.Width = _userSettings.WindowWidth;
                _window.Height = _userSettings.WindowHeight;
            }
            else
            {
                // Default to primary screen if the saved position is not valid
                var primaryScreen = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                _window.Left = primaryScreen.Left;
                _window.Top = primaryScreen.Top;
                _window.Width = _userSettings.WindowWidth;
                _window.Height = _userSettings.WindowHeight;
            }
        }

    }
}
