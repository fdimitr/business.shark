using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BusinessSharkClient.View.Divisions;

public partial class SawmillListView : ContentView, INotifyPropertyChanged
{
    private ObservableCollection<SawmillListGroup> _groupedSawmills = new ObservableCollection<SawmillListGroup>
    {
            new SawmillListGroup("Loading...", "loading.png", new List<SawmillListModel>
            {
                new SawmillListModel { Name = "Please wait", ProductName = "Loading data...", Volume = "" }
            })
    };

    public ObservableCollection<SawmillListGroup> GroupedSawmills
    {
        get => _groupedSawmills;
        set
        {
            if (_groupedSawmills != value)
            {
                _groupedSawmills = value;
                OnPropertyChanged();
            }
        }
    }

    private SawmillProvider _sawmillProvider;

    public SawmillListView(SawmillProvider sawmillProvider)
    {
        _sawmillProvider = sawmillProvider;
        InitializeComponent();

        Loaded += OnLoadingView;

        BindingContext = this;
     }

    public async void OnLoadingView(object? sender, EventArgs e)
    {
        GroupedSawmills = await _sawmillProvider.LoadData(int.Parse(await SecureStorage.Default.GetAsync("company_id") ?? "0"));
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}