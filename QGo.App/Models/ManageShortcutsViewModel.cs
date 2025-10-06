using QGo.App.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace QGo;
public sealed class ManageShortcutsViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Shortcut> Items { get; }
    public Shortcut Selected { get; set; }

    public ManageShortcutsViewModel(IEnumerable<Shortcut> initial)
        => Items = new ObservableCollection<Shortcut>(initial.Select(s => new Shortcut { Key = s.Key, Template = s.Template }));

    public void Add() => Items.Add(new Shortcut { Key = "new", Template = "" });
    public void RemoveSelected() { if (Selected != null) Items.Remove(Selected); }

    public event PropertyChangedEventHandler PropertyChanged;
}
