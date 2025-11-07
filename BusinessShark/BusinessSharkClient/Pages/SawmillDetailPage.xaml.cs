using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.ViewModels;
using BusinessSharkClient.Pages.Finance;
using System.Windows.Input;

namespace BusinessSharkClient.Pages;

public partial class SawmillDetailPage : ContentPage
{
    public ICommand OpenDivisionAnalyticsCommand { get; }
    public SawmillDetailViewModel SawmillDetail { get; set; }


    private SawmillProvider _sawmillProvider;
    private GlobalDataProvider _globalDataProvider;
    private int _divisionId;

    public SawmillDetailPage(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, int divisionId)
    {
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        _divisionId = divisionId;
        SawmillDetail = new SawmillDetailViewModel(_globalDataProvider, _sawmillProvider) { Name = "Loading ..." };

        OpenDivisionAnalyticsCommand = new Command(OnOpenDivisionAnalytics);

        InitializeComponent();
        Loaded += OnLoadingView;

        BindingContext = this;
        DataStackLayout.BindingContext = SawmillDetail;
    }

    private void OnOpenDivisionAnalytics(object obj)
    {
        Navigation.PushAsync(new DivisionAnalyticsPage());
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