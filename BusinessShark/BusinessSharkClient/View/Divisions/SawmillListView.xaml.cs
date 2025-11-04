using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.Models;
using BusinessSharkClient.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BusinessSharkClient.View.Divisions;

public partial class SawmillListView : ContentView, INotifyPropertyChanged
{
    public ICommand OpenDetailsCommand { get; }

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
    private GlobalDataProvider _globalDataProvider;

    public SawmillListView(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider)
    {
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        InitializeComponent();

        OpenDetailsCommand = new Command<SawmillListModel>(OnOpenDetails);
        Loaded += OnLoadingView;

        BindingContext = this;
     }

    public async void OnLoadingView(object? sender, EventArgs e)
    {
        GroupedSawmills = await _sawmillProvider.LoadList(int.Parse(await SecureStorage.Default.GetAsync("company_id") ?? "0"));
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async void OnOpenDetails(SawmillListModel sawmill)
    {
        await Navigation.PushAsync(new SawmillDetailPage(_globalDataProvider, _sawmillProvider, sawmill.Id));
    }
}