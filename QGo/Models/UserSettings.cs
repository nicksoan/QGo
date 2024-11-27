using Newtonsoft.Json;
using System.IO;
using System.Windows.Input;

namespace QGo.Models
{
    public class UserSettings
    {
        public string WindowColour { get; set; } = "#FFFFFF";
        public string FoundColour { get; set; } = "#FFFFFF";
        public string FontColour { get; set; } = "#000000";
        public string FontColourFound { get; set; } = "#FFFF00";

        public double FontSize { get; set; } = 12.0;
        public double WindowPositionX { get; set; } = 100.0;
        public double WindowPositionY { get; set; } = 100.0;

        public double WindowWidth { get; set; } = 150;
        public double WindowHeight { get; set; } = 50;

        public Key HotKey { get; set; } = Key.Q;
        public List<ModifierKeys> HotKeyModifiers { get; set; } = new List<ModifierKeys> { ModifierKeys.Alt };


        private readonly string _settingsFilePath = "";

        // Load settings from JSON file
        public UserSettings(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;

            string json = File.ReadAllText(_settingsFilePath);
            JsonConvert.PopulateObject(json, this );
        }

        // Save settings to JSON file
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath));
            File.WriteAllText(_settingsFilePath, json);
        }
    }
}
