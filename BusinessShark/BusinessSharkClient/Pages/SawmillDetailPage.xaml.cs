using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.ViewModels;
using BusinessSharkClient.Pages.Finance;
using System.Windows.Input;

namespace BusinessSharkClient.Pages;

public partial class SawmillDetailPage : ContentPage
{
    public ICommand OpenDivisionAnalyticsCommand { get; }
    public ICommand OpenFinancialStatisticsCommand { get; }
    public ICommand OpenDivisionWarehouseCommand { get; }

    public SawmillDetailViewModel SawmillDetail { get; set; }

    private readonly DivisionTransactionProvider _transactionProvider;
    private readonly DivisionWarehouseProvider _warehouseProvider;
    private readonly GlobalDataProvider _globalDataProvider;
    private readonly int _divisionId;

    public SawmillDetailPage(GlobalDataProvider globalDataProvider, 
        SawmillProvider sawmillProvider, 
        DivisionTransactionProvider transactionProvider,
        DivisionWarehouseProvider warehouseProvider,
        int divisionId)
    {
        _transactionProvider = transactionProvider;
        _warehouseProvider = warehouseProvider;
        _globalDataProvider = globalDataProvider;
        _divisionId = divisionId;
        SawmillDetail = new SawmillDetailViewModel(globalDataProvider, sawmillProvider) { Name = "Loading ..." };

        OpenDivisionAnalyticsCommand = new Command(OnOpenDivisionAnalytics);
        OpenFinancialStatisticsCommand = new Command(OnOpenFinancialStatistics);
        OpenDivisionWarehouseCommand = new Command(OnOpenDivisionWarehouse);

        InitializeComponent();
        Loaded += OnLoadingView;

        BindingContext = this;
        DataStackLayout.BindingContext = SawmillDetail;
    }

    private void OnOpenDivisionWarehouse(object obj)
    {
        Navigation.PushAsync(new DivisionWarehousePage(_warehouseProvider, _globalDataProvider, _divisionId));
    }

    private void OnOpenFinancialStatistics(object obj)
    {
        Navigation.PushAsync(new FinancialStatisticsPage(_transactionProvider, _divisionId));
    }

    private void OnOpenDivisionAnalytics(object obj)
    {
        Navigation.PushAsync(new DivisionAnalyticsPage(_transactionProvider, _divisionId));
    }

    private async void OnLoadingView(object? sender, EventArgs e)
    {
        try
        {
            await SawmillDetail.LoadAsync(_divisionId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }
}
