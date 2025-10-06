using QGo.App.Models;

namespace QGo;
public sealed class SettingsViewModel
{
    public AppSettings Settings { get; }
    public SettingsViewModel(AppSettings settings) => Settings = settings;
}
