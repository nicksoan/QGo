using QGo.App.Models;
using System.IO;
using System.Text.Json;

namespace QGo.App;
public static class Storage
{
    static readonly JsonSerializerOptions Opt = new() { WriteIndented = true };

    // Final base path — includes the "Data" subfolder
    static string BaseDir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "QGo", "Data");

    public static string LinksPath => Path.Combine(BaseDir, "links.json");
    static string SettingsPath => Path.Combine(BaseDir, "settings.json");

    public static Dictionary<string, string> LoadLinksOrDefaults()
    {
        EnsureDir();
        if (File.Exists(LinksPath))
            return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(LinksPath)) ?? new();

        var defaults = new Dictionary<string, string>
        {
            ["google"] = "https://www.google.com/search?q={param}",
            ["documents"] = "%HOMEPATH%",
            ["networkshare"] = @"\\server\sharedfolder",
            ["youtube"] = "https://www.youtube.com/results?search_query={param}",
            ["reddit"] = "https://reddit.com",
            ["github"] = "https://github.com/",
            ["temp"] = @"C:\\temp",
            ["qgo"] = "https://github.com/nicksoan/QGo",
            ["repo"] = "%USERPROFILE%",
            ["notepad"] = @"C:\\Program Files (x86)\\Notepad++\\notepad++.exe"
        };

        SaveLinks(defaults.Select(kv => new Shortcut { Key = kv.Key, Template = kv.Value }));
        return defaults;
    }

    public static void SaveLinks(IEnumerable<Shortcut> items)
    {
        EnsureDir();
        var dict = items.ToDictionary(s => s.Key, s => s.Template);
        File.WriteAllText(LinksPath, JsonSerializer.Serialize(dict, Opt));
    }

    public static AppSettings LoadSettings()
    {
        EnsureDir();
        if (File.Exists(SettingsPath))
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new();

        var s = new AppSettings();
        SaveSettings(s);
        return s;
    }

    public static void SaveSettings(AppSettings s)
    {
        EnsureDir();
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(s, Opt));
    }

    private static void EnsureDir() => Directory.CreateDirectory(BaseDir);
}
