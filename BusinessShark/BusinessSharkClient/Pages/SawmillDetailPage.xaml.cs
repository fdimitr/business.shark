using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.ViewModels;
using BusinessSharkClient.Pages.Finance;
using System.Windows.Input;

namespace BusinessSharkClient.Pages;

public partial class SawmillDetailPage : ContentPage
{
    public ICommand OpenDivisionAnalyticsCommand { get; }
    public ICommand OpenFinancialStatisticsCommand { get; }
    public ICommand OpenProductionStatisticsCommand { get; }
    public ICommand OpenSalesCommand { get; }

    public SawmillDetailViewModel SawmillDetail { get; set; }


    private DivisionTransactionProvider _transactionProvider;
    private int _divisionId;

    public SawmillDetailPage(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, DivisionTransactionProvider transactionProvider, int divisionId)
    {
        _transactionProvider = transactionProvider;
        _divisionId = divisionId;
        SawmillDetail = new SawmillDetailViewModel(globalDataProvider, sawmillProvider) { Name = "Loading ..." };

        OpenDivisionAnalyticsCommand = new Command(OnOpenDivisionAnalytics);
        OpenFinancialStatisticsCommand = new Command(OnOpenFinancialStatistics);
        OpenProductionStatisticsCommand = new Command(OnOpenProductionStatistics);
        OpenSalesCommand = new Command(OnOpenSales);

        InitializeComponent();
        Loaded += OnLoadingView;

        BindingContext = this;
        DataStackLayout.BindingContext = SawmillDetail;
    }

    private void OnOpenSales(object obj)
    {

    }

    private void OnOpenProductionStatistics(object obj)
    {

    }

    private void OnOpenFinancialStatistics(object obj)
    {

    }

    private void OnOpenDivisionAnalytics(object obj)
    {
        Navigation.PushAsync(new DivisionAnalyticsPage(_transactionProvider, _divisionId));
    }

    private async void OnLoadingView(object? sender, EventArgs e)
    {
        await SawmillDetail.LoadAsync(_divisionId);
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Check if we can go back
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
    }
}
