using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.Models;

namespace BusinessSharkClient.View;

public partial class ProductDefinitionView : ContentPage
{
    public ProductDefinitionModel CurrentProduct { get; set; }

    private GlobalDataProvider _globalDataProvider;

    public ProductDefinitionView(ProductDefinitionModel product, GlobalDataProvider globalDataProvider)
    {
        InitializeComponent();
        _globalDataProvider = globalDataProvider;
        CurrentProduct = AddDataToComponents(product, globalDataProvider);

        if (CurrentProduct.ComponentUnits == null || CurrentProduct.ComponentUnits.Count == 0)
        {
            ComponentHeader.IsVisible = false;
        }

        // Set the BindingContext of the page to the product
        // This makes all the {Binding ...} expressions in XAML work
        this.BindingContext = CurrentProduct;
    }

    public ProductDefinitionModel AddDataToComponents(ProductDefinitionModel product, GlobalDataProvider globalDataProvider)
    {
        foreach (var component in product.ComponentUnits)
        {
            if (string.IsNullOrEmpty(component.Name))
            {
                var detailedComponent = globalDataProvider.ProductDefinitions
                    .FirstOrDefault(p => p.ProductDefinitionId == component.ComponentDefinitionId);
                if (detailedComponent != null)
                {
                    component.Name = detailedComponent.Name;
                    component.Image = detailedComponent.Image;
                }
            }
        }
        return product;
    }

    /// <summary>
    /// Handles the click event for the custom back button.
    /// Navigates to the previous page in the stack.
    /// </summary>
    private async void OnBackButtonClicked(object sender, System.EventArgs e)
    {
        // Check if we can go back
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
    }

    /// <summary>
    /// Handles the selection of a component from the CollectionView.
    /// Navigates to a new instance of this same page, passing the selected component.
    /// </summary>
    private async void OnComponentSelected(object sender, SelectionChangedEventArgs e)
    {
        // Get the selected component (which is also a Product)
        if (e.CurrentSelection.FirstOrDefault() is ComponentUnitModel selectedComponent)
        {
            // Navigate to a new ProductDetailPage, passing the selected component
            // This creates the drill-down behavior you wanted

            var definition = _globalDataProvider.ProductDefinitions.FirstOrDefault(p => p.ProductDefinitionId == selectedComponent.ComponentDefinitionId);
            if (definition != null)
            {
                await Navigation.PushAsync(new ProductDefinitionView(definition, _globalDataProvider));
            }

            // Deselect the item in the list
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}