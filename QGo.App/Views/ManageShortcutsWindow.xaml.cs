using Microsoft.Win32;
using QGo.App;
using QGo.App.Models;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace QGo.App.Views;
public partial class ManageShortcutsWindow : Window
{
    private static readonly JsonSerializerOptions JsonOpt = new() { WriteIndented = true };

    public ManageShortcutsWindow() { InitializeComponent(); }
    void Add_Click(object _, RoutedEventArgs __)
        => (DataContext as ManageShortcutsViewModel)?.Add();
    void Remove_Click(object _, RoutedEventArgs __)
        => (DataContext as ManageShortcutsViewModel)?.RemoveSelected();
    void Save_Click(object _, RoutedEventArgs __)
    {
        if (DataContext is ManageShortcutsViewModel vm)
        {
            Storage.SaveLinks(vm.Items);
            DialogResult = true;
        }
    }
    void Import_Click(object _, RoutedEventArgs __)
    {
        if (DataContext is not ManageShortcutsViewModel vm) return;

        var dlg = new OpenFileDialog
        {
            Title = "Import Shortcuts",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = System.IO.Path.GetDirectoryName(Storage.LinksPath)
        };
        if (dlg.ShowDialog(this) != true) return;

        try
        {
            var json = File.ReadAllText(dlg.FileName);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();

            // replace current items with imported
            vm.Items.Clear();
            foreach (var kv in dict.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                vm.Items.Add(new Shortcut { Key = kv.Key, Template = kv.Value });

            // optionally persist immediately
            Storage.SaveLinks(vm.Items);
            MessageBox.Show(this, "Shortcuts imported.", "QGo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Import failed:\n{ex.Message}", "QGo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void Export_Click(object _, RoutedEventArgs __)
    {
        if (DataContext is not ManageShortcutsViewModel vm) return;

        var dlg = new SaveFileDialog
        {
            Title = "Export Shortcuts",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = "links.json",
            InitialDirectory = System.IO.Path.GetDirectoryName(Storage.LinksPath)
        };
        if (dlg.ShowDialog(this) != true) return;

        try
        {
            var dict = vm.Items.ToDictionary(s => s.Key, s => s.Template, StringComparer.OrdinalIgnoreCase);
            File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(dict, JsonOpt));
            MessageBox.Show(this, "Shortcuts exported.", "QGo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Export failed:\n{ex.Message}", "QGo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
