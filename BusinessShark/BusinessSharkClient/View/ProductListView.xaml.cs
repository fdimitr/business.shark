using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.Models;


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
        if (MainContainer.Children.Count == 0)
        {
            var dataSource = _globalDataProvider.ProductDefinitions.GroupBy(p => p.ProductCategoryId).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var category in dataSource)
            {
                var categoryName = _globalDataProvider.ProductCategories.FirstOrDefault(c => c.ProductCategoryId == category.Key)?.Name ?? "Uncategorized";
                var categoryTable = CreateCategoryTable(categoryName, category.Value);
                MainContainer.Children.Add(categoryTable);
            }
        }
    }
    private IView CreateCategoryTable(string categoryName, List<ProductDefinitionModel> productDefinitions)
    {
        var container = new VerticalStackLayout
        {
            Spacing = 5
        };

        // Category Header
        var header = new Label
        {
            Text = categoryName,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#3560A0"),
            Margin = new Thickness(0, 5, 0, 10),
            HorizontalTextAlignment = TextAlignment.Center
        };
        container.Children.Add(header);

        // Table with ProductDefinitions
        var grid = new Grid
        {
            ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star)
                },
            RowSpacing = 0,
            ColumnSpacing = 10,
        };

        int row = 0;

        var productPairs = productDefinitions.Chunk(2);
        foreach (var pair in productPairs)
        {
            var rowBackground = new Grid
            {
                ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
                BackgroundColor = (row % 2 == 1) ? Color.FromArgb("#F2F2F2") : Colors.Transparent,
                Padding = new Thickness(4, 6)
            };

            int col = 0;

            foreach (var product in pair)
            {
                // Product wrapper (so that the click works for everything)
                var productLayout = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                var image = new Image
                {
                    Source = product.Image,
                    HeightRequest = 40,
                    WidthRequest = 40,
                    Aspect = Aspect.AspectFit
                };

                var name = new Label
                {
                    Text = product.Name,
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Center
                };

                // Add TapGestureRecognizer
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => OnProductClicked(product);
                image.GestureRecognizers.Add(tap);
                name.GestureRecognizers.Add(tap);

                productLayout.Children.Add(image);
                productLayout.Children.Add(name);

                // Add to Row
                rowBackground.Add(productLayout, col, 0);
                rowBackground.SetColumnSpan(productLayout, 2);

                col += 2;
            }

            // Add to Category Grid
            grid.Add(rowBackground, 0, row);
            grid.SetColumnSpan(rowBackground, 4);

            row++;
        }

        container.Children.Add(grid);
        return container;
    }

    private async void OnProductClicked(ProductDefinitionModel product)
    {
         await Navigation.PushAsync(new ProductDefinitionView(product, _globalDataProvider));
    }
}