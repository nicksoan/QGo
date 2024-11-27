using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace QGo.Windows
{
    /// <summary>
    /// Interaction logic for EditShortcutsWindow.xaml
    /// </summary>
    public partial class EditShortcutsWindow : Window
    {
        private readonly string _filePath;
        private Dictionary<string, string> shortcuts;
        private MainWindow mainWindow;
        public EditShortcutsWindow(string filePath, MainWindow mainWindow)
        {
            _filePath = filePath;
            InitializeComponent();
            LoadShortcuts();
            InitializePlaceholders();
            this.mainWindow = mainWindow;
        }

        private void LoadShortcuts()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                shortcuts = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            else
            {
                shortcuts = new Dictionary<string, string>();
            }
            RefreshListView();
        }

        private void RefreshListView()
        {
            ShortcutsListView.ItemsSource = shortcuts.Select(kvp => new Models.Shortcut { Key = kvp.Key, Value = kvp.Value }).ToList();
        }

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
            var key = GetText(KeyTextBox);
            var value = GetText(ValueTextBox);

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Key and Value cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (shortcuts.ContainsKey(key))
            {
                MessageBox.Show("Key already exists. Use Update to modify it.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            shortcuts[key] = value;
            SaveShortcuts();
            RefreshListView();
            ClearInputs();
            mainWindow.UpdateUserShortcuts();
            
        }

        private void UpdateShortcut_Click(object sender, RoutedEventArgs e)
        {
            var key = GetText(KeyTextBox);
            var value = GetText(ValueTextBox);

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Key and Value cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!shortcuts.ContainsKey(key))
            {
                MessageBox.Show("Key does not exist. Use Add to create a new shortcut.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            shortcuts[key] = value.ToLower();
            SaveShortcuts();
            RefreshListView();
            ClearInputs();
            mainWindow.UpdateUserShortcuts();
        }

        private void DeleteShortcut_Click(object sender, RoutedEventArgs e)
        {
            var key = GetText(KeyTextBox);

            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Key cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!shortcuts.ContainsKey(key))
            {
                MessageBox.Show("Key does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            shortcuts.Remove(key);
            SaveShortcuts();
            RefreshListView();
            ClearInputs();
            mainWindow.UpdateUserShortcuts();
        }

        private void SaveShortcuts()
        {
            var json = JsonSerializer.Serialize(shortcuts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private void ClearInputs()
        {
            SetPlaceholder(KeyTextBox);
            SetPlaceholder(ValueTextBox);
        }

        // Placeholder Handling
        private void InitializePlaceholders()
        {
            SetPlaceholder(KeyTextBox);
            SetPlaceholder(ValueTextBox);

            KeyTextBox.GotFocus += RemovePlaceholder;
            KeyTextBox.LostFocus += ApplyPlaceholder;

            ValueTextBox.GotFocus += RemovePlaceholder;
            ValueTextBox.LostFocus += ApplyPlaceholder;
        }

        private void SetPlaceholder(System.Windows.Controls.TextBox textBox)
        {
            textBox.Text = textBox.Tag.ToString();
            textBox.Foreground = Brushes.Gray;
        }

        private string GetText(TextBox textBox)
        {
            return textBox.Foreground == Brushes.Gray ? string.Empty : textBox.Text.Trim();
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Foreground == Brushes.Gray)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void ApplyPlaceholder(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholder(textBox);
            }
        }

        private void ShortcutsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShortcutsListView.SelectedItem is Models.Shortcut selectedItem)
            {
                KeyTextBox.Text = selectedItem.Key;
                ValueTextBox.Text = selectedItem.Value;
                KeyTextBox.Foreground = Brushes.Black;
                ValueTextBox.Foreground = Brushes.Black;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportShortcuts_Click(object sender, RoutedEventArgs e)
        {
             var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "shortcuts.json"
            };


            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var json = JsonSerializer.Serialize(shortcuts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveFileDialog.FileName, json);
                MessageBox.Show("Shortcuts exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportShortcuts_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var json = File.ReadAllText(openFileDialog.FileName);
                var importedShortcuts = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (importedShortcuts != null)
                {
                    shortcuts = importedShortcuts;
                    SaveShortcuts();
                    RefreshListView();
                    MessageBox.Show("Shortcuts imported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to import shortcuts.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
