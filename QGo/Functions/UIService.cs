using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using TextBox = System.Windows.Controls.TextBox;

namespace QGo.Functions
{
    public class UIService
    {
        private readonly TextBox _textBox;
        private readonly List<char> _currentCharsList;
        private readonly TextChangedEventHandler _textChangedHandler;
        private SolidColorBrush _defaultColour = new SolidColorBrush(Colors.White);
        private SolidColorBrush _errorColour = new SolidColorBrush(Colors.LightPink);
        private int _previousTextLength = 0;

        public UIService(TextBox textBox, List<char> currentCharsList, TextChangedEventHandler textChangedHandler, SolidColorBrush? defaultColour, int previousTextLength)
        {
            _textBox = textBox;
            _currentCharsList = currentCharsList;
            _textChangedHandler = textChangedHandler;

            if (defaultColour != null)
            {
                _defaultColour = defaultColour;
            }
            _previousTextLength = previousTextLength;
        }

        public void ClearText()
        {
            _textBox.TextChanged -= _textChangedHandler;

            _textBox.Clear();
            _currentCharsList.Clear();

            _textBox.TextChanged += _textChangedHandler;

            SetDefaultBGColour();
        }

        public void SetDefaultBGColour()
        {
            _textBox.Background = _defaultColour;
        }

        public void CheckClearReset()
        {
            if (_textBox.Text.Length == 0)
            {
                _currentCharsList.Clear();
                _previousTextLength = 0;
                SetDefaultBGColour();
            }
        }

        public void FlashError( int durationMs = 500, int intervalMs = 150)
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
                    SetDefaultBGColour();
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
    }
}
