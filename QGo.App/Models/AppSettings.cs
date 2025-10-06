using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QGo.App.Models
{
    public sealed class AppSettings
    {
        public string HotkeyGesture { get; set; } = "Alt+Escape"; // e.g., Alt+Shift+G
        public string Background { get; set; } = "#CC1E1E1E";
        public string Foreground { get; set; } = "#000000";
        public double FontSize { get; set; } = 18;
        public string FontFamily { get; set; } = "Segoe UI";
    }
}
