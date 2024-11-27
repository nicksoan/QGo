using QGo.Models;
using System.Runtime;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;

namespace QGo.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private UserSettings _settings;
        private MainWindow mainWindow;
        private System.Drawing.Color selectedColor;
        private List<Key> _pressedKeys = new List<Key>();

        public Settings(UserSettings userSettings, MainWindow mainWindow)
        {
            InitializeComponent();
            _settings = userSettings;

            // Load settings into controls
            txtWindowColor.Text = _settings.WindowColour;
            txtWindowColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_settings.WindowColour));

            txtFontSize.Text = _settings.FontSize.ToString();
            txtFontSize.FontSize = (int)_settings.FontSize;

            txtFoundMatch.Text = _settings.FoundColour;
            txtFoundMatch.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_settings.FoundColour));

            txtFontColour.Text = _settings.FontColour;
            txtFontColour.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_settings.FontColour));

            txtFontColourFound.Text = _settings.FontColourFound;
            txtFontColourFound.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_settings.FontColourFound));

            if (_settings.HotKeyModifiers.Count == 0)
            {
                _settings.HotKeyModifiers.Add(ModifierKeys.Alt);
            }

            // Join all modifiers and display them along with the hotkey
            txtShortcut.Text = $"{string.Join(" + ", _settings.HotKeyModifiers)} + {_settings.HotKey}";

            this.mainWindow = mainWindow;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update settings from controls
                _settings.WindowColour = txtWindowColor.Text;
                _settings.FontColour = txtFontColour.Text;

                _settings.FoundColour = txtFoundMatch.Text;
                _settings.FontColourFound = txtFontColourFound.Text;

                _settings.FontSize = double.Parse(txtFontSize.Text);

                _settings.Save();

                mainWindow.UpdateUserSettings(_settings);

                //MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnWindowColor_Click(object sender, RoutedEventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = selectedColor;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    var colour = new SolidColorBrush(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B));
                    txtWindowColor.Background = colour;
                    txtWindowColor.Text = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                }
            }
        }

        private void btnFontColour_Click(object sender, RoutedEventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = selectedColor;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    var colour = new SolidColorBrush(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B));
                    txtFontColour.Background = colour;
                    txtFontColour.Text = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                }
            }
        }

        private void btnFoundMatch_Click(object sender, RoutedEventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = selectedColor;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    var colour = new SolidColorBrush(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B));
                    txtFoundMatch.Background = colour;
                    txtFoundMatch.Text = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                }
            }
        }

        private void btnFontColourFound_Click(object sender, RoutedEventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = selectedColor;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    var colour = new SolidColorBrush(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B));
                    txtFontColourFound.Background = colour;
                    txtFontColourFound.Text = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                }
            }
        }

        private void btnRecordShortcut_Click(object sender, RoutedEventArgs e)
        {
            btnRecordShortcut.IsEnabled = false;
            _pressedKeys.Clear();
            txtShortcut.Text = "Press the desired shortcut...";
            txtShortcut.Focus();
            txtShortcut.KeyDown += txtShortcut_KeyDown;
            txtShortcut.KeyUp += txtShortcut_KeyUp;
        }

        private void txtShortcut_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle the System key (Alt key)
            if (e.SystemKey != Key.None)
            {
                if (!_pressedKeys.Contains(e.SystemKey))
                {
                    _pressedKeys.Add(e.SystemKey);
                }
            }
            else
            {
                if (!_pressedKeys.Contains(e.Key))
                {
                    _pressedKeys.Add(e.Key);
                }
            }

            UpdateShortcutText();
            e.Handled = true; // Prevent further processing of the key event
        }

        private void txtShortcut_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            txtShortcut.KeyDown -= txtShortcut_KeyDown;
            txtShortcut.KeyUp -= txtShortcut_KeyUp;

            SaveShortcut();

            btnRecordShortcut.IsEnabled = true;

            if (_settings.HotKeyModifiers.Count == 0)
            {
                MessageBox.Show("Please create a valid shortcut using a combination of one or more modifier keys (Control, Alt, Shift, Windows) plus a hotkey.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void UpdateShortcutText()
        {
            var modifiers = Keyboard.Modifiers;
            var keys = new List<string>();

            if (modifiers.HasFlag(ModifierKeys.Control) && !keys.Contains("Control"))
                keys.Add("Control");
            if (modifiers.HasFlag(ModifierKeys.Alt) && !keys.Contains("Alt"))
                keys.Add("Alt");
            if (modifiers.HasFlag(ModifierKeys.Shift) && !keys.Contains("Shift"))
                keys.Add("Shift");
            if (modifiers.HasFlag(ModifierKeys.Windows) && !keys.Contains("Windows"))
                keys.Add("Windows");

            foreach (var key in _pressedKeys)
            {
                if (key != Key.LeftCtrl && key != Key.RightCtrl &&
                    key != Key.LeftAlt && key != Key.RightAlt &&
                    key != Key.LeftShift && key != Key.RightShift &&
                    key != Key.LWin && key != Key.RWin)
                {
                    keys.Add(key.ToString());
                }
            }

            txtShortcut.Text = string.Join(" + ", keys);
        }

        private void SaveShortcut()
        {
            var modifiers = Keyboard.Modifiers;
            var key = _pressedKeys.Count > 0 ? _pressedKeys[_pressedKeys.Count - 1] : Key.None;

            // Assuming you have a UserSettings instance named _settings
            _settings.HotKey = key;
            _settings.HotKeyModifiers = new List<ModifierKeys>();

            if (modifiers.HasFlag(ModifierKeys.Control) && !_settings.HotKeyModifiers.Contains(ModifierKeys.Control))
                _settings.HotKeyModifiers.Add(ModifierKeys.Control);
            if (modifiers.HasFlag(ModifierKeys.Alt) && !_settings.HotKeyModifiers.Contains(ModifierKeys.Alt))
                _settings.HotKeyModifiers.Add(ModifierKeys.Alt);
            if (modifiers.HasFlag(ModifierKeys.Shift) && !_settings.HotKeyModifiers.Contains(ModifierKeys.Shift))
                _settings.HotKeyModifiers.Add(ModifierKeys.Shift);
            if (modifiers.HasFlag(ModifierKeys.Windows) && !_settings.HotKeyModifiers.Contains(ModifierKeys.Windows))
                _settings.HotKeyModifiers.Add(ModifierKeys.Windows);

            // Save settings to file
            _settings.Save();
        }
    }
}
