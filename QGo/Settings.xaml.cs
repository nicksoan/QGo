using QGo.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;

namespace QGo
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private UserSettings settings;
        private MainWindow mainWindow;
        private System.Drawing.Color selectedColor;

        public Settings(UserSettings userSettings, MainWindow mainWindow)
        {
            InitializeComponent();
            settings = userSettings;

            // Load settings into controls
            txtWindowColor.Text = settings.WindowColour;
            txtWindowColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.WindowColour));

            txtFontSize.Text = settings.FontSize.ToString();
            txtFontSize.FontSize = (int)settings.FontSize;

            txtFoundMatch.Text = settings.FoundColour;
            txtFoundMatch.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.FoundColour));
            
            txtFontColour.Text = settings.FontColour;
            txtFontColour.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.FontColour));

            txtFontColourFound.Text = settings.FontColourFound;
            txtFontColourFound.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.FontColourFound));


            this.mainWindow = mainWindow;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update settings from controls
                settings.WindowColour = txtWindowColor.Text;
                settings.FontColour = txtFontColour.Text;

                settings.FoundColour = txtFoundMatch.Text;
                settings.FontColourFound = txtFontColourFound.Text;

                settings.FontSize = double.Parse(txtFontSize.Text);

                settings.Save();

                mainWindow.UpdateUserSettings(settings);

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
    }
}
