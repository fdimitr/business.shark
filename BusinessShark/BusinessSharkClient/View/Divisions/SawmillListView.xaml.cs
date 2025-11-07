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
    private DivisionTransactionProvider _transactionProvider;

    public SawmillListView(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, DivisionTransactionProvider transactionProvider)
    {
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        _transactionProvider = transactionProvider;
        InitializeComponent();

        OpenDetailsCommand = new Command<SawmillListModel>(OnOpenDetails);
        Loaded += OnLoadingView;

        BindingContext = this;
        _transactionProvider = transactionProvider;
    }

    public async void OnLoadingView(object? sender, EventArgs e)
    {
        try
        {
            GroupedSawmills = await _sawmillProvider.LoadList(int.Parse(await SecureStorage.Default.GetAsync("company_id") ?? "0"));
        }
        catch (Exception ex)
        {
            if (this.Parent is ContentPage page)
                await page.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async void OnOpenDetails(SawmillListModel sawmill)
    {
        try
        {
            var sawmillDetailPage = new SawmillDetailPage(_globalDataProvider, _sawmillProvider, _transactionProvider, sawmill.Id);
            await Navigation.PushAsync(sawmillDetailPage);
        }
        catch (Exception ex)
        {
            if (this.Parent is ContentPage page)
                await page.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }
}