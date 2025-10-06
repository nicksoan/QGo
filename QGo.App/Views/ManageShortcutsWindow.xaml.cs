using QGo.App;
using System.Windows;

namespace QGo.App.Views;
public partial class ManageShortcutsWindow : Window
{
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
}
