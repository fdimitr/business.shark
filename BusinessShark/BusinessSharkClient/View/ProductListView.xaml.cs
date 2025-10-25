
using BusinessSharkClient.Logic;

namespace BusinessSharkClient.View;

public partial class ProductListView : ContentPage
{
    private GlobalDataProvider _globalDataProvider;

    public ProductListView(GlobalDataProvider globalDataProvider)
	{
        InitializeComponent();

        _globalDataProvider = globalDataProvider;
        Loaded += LoadPageData;
    }

    private void LoadPageData(object? sender, EventArgs e)
    {
        ProductList.ItemsSource = _globalDataProvider.ProductDefinitions;
    }
}