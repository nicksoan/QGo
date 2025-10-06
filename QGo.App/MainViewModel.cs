using QGo.App;
using QGo.App.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace QGo;
public sealed class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Shortcut> All { get; } = new();
    public ICollectionView FilteredItems { get; }

    public AppSettings Settings { get; private set; } = Storage.LoadSettings();

    public Brush BackgroundBrush { get; private set; }
    public Brush ForegroundBrush { get; private set; }

    string _searchText = "";
    public string SearchText {
        get => _searchText; 
        set { 
            if (_searchText == value) return; 
            _searchText = value; 
            OnPropertyChanged();
            FilteredItems.Refresh(); 
        } 
    }

    Shortcut _selected;
    public Shortcut Selected { get => _selected; set { if (_selected == value) return; _selected = value; OnPropertyChanged(); } }
    public bool IsListVisible =>
    !string.IsNullOrWhiteSpace(SearchText) &&
    FilteredItems?.Cast<object>().Any() == true;

    public MainViewModel()
    {
        foreach (var kv in Storage.LoadLinksOrDefaults())
            All.Add(new Shortcut { Key = kv.Key, Template = kv.Value });

        FilteredItems = CollectionViewSource.GetDefaultView(All);
        FilteredItems.Filter = o =>
        {
            if (o is not Shortcut s) return false;
            var q = SearchText?.Trim() ?? "";
            if (q.Length == 0) return true;
            var i = q.IndexOf(' ');
            var token = i < 0 ? q : q[..i];
            return s.Key.StartsWith(q, StringComparison.OrdinalIgnoreCase)
                || s.Key.Contains(token, StringComparison.OrdinalIgnoreCase);
        };

        if (FilteredItems is System.Collections.Specialized.INotifyCollectionChanged cc)
            cc.CollectionChanged += (_, __) => OnPropertyChanged(nameof(IsListVisible));

        ApplyTheme();
    }

    public void ApplyTheme()
    {
        var bc = new BrushConverter();
        BackgroundBrush = (Brush)bc.ConvertFromString(Settings.Background);
        ForegroundBrush = (Brush)bc.ConvertFromString(Settings.Foreground);
        OnPropertyChanged(nameof(BackgroundBrush));
        OnPropertyChanged(nameof(ForegroundBrush));
        OnPropertyChanged(nameof(Settings)); // font bindings pick up changes
    }

    public void ReloadFrom(IEnumerable<Shortcut> items)
    {
        All.Clear();
        foreach (var s in items.OrderBy(s => s.Key, StringComparer.OrdinalIgnoreCase))
            All.Add(s);
        FilteredItems.Refresh();
    }

    public void LaunchSelected()
    {
        var input = (SearchText ?? "").Trim();

        // split key and param
        string key = input, param = "";
        var i = input.IndexOf(' ');
        if (i >= 0) { key = input[..i]; param = input[(i + 1)..].Trim(); }

        // 1) if user has a selection, honor it
        var chosen = Selected ?? (FilteredItems?.CurrentItem as Shortcut);

        // 2) else fall back to exact key, then startswith
        if (chosen == null && !string.IsNullOrEmpty(key))
            chosen = All.FirstOrDefault(t => t.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                  ?? All.FirstOrDefault(t => t.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase));

        if (chosen == null) return;

        var target = Expand(chosen.Template, param);
        TryOpen(target, param);
    }

    static string Expand(string template, string param)
    {
        var encoded = Uri.EscapeDataString(param ?? "");
        var expanded = Environment.ExpandEnvironmentVariables(template ?? "");
        return expanded.Replace("{param}", encoded);
    }

    static void TryOpen(string target, string rawParam)
    {
        if (string.IsNullOrWhiteSpace(target)) return;

        if (Uri.TryCreate(target, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        { Process.Start(new ProcessStartInfo(target) { UseShellExecute = true }); return; }

        if (File.Exists(target) && Path.GetExtension(target).Equals(".exe", StringComparison.OrdinalIgnoreCase))
        { Process.Start(new ProcessStartInfo(target, rawParam) { UseShellExecute = true }); return; }

        if (Directory.Exists(target))
        { Process.Start(new ProcessStartInfo("explorer.exe", $"\"{target}\"") { UseShellExecute = true }); return; }

        if (File.Exists(target))
        { Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{target}\"") { UseShellExecute = true }); return; }

        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}