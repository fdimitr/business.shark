using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.ViewModels;

namespace BusinessSharkClient.Pages;

public partial class SawmillDetailPage : ContentPage
{
    public SawmillDetailViewModel SawmillDetail { get; set; }


    private SawmillProvider _sawmillProvider;
    private GlobalDataProvider _globalDataProvider;
    private int _divisionId;

    public SawmillDetailPage(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, int divisionId)
	{
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        _divisionId = divisionId;

        InitializeComponent();
        Loaded += OnLoadingView;

        SawmillDetail = new SawmillDetailViewModel(_globalDataProvider, _sawmillProvider) { Name = "Loading ..." };
        BindingContext = this;
        DataStackLayout.BindingContext = SawmillDetail;
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