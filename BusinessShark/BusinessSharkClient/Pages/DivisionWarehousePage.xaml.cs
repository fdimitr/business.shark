using System.Windows.Input;
using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.Models;

namespace BusinessSharkClient.Pages;

public partial class DivisionWarehousePage : ContentPage
{
    public ICommand UpdateModelCommand { get; }

    public DivisionWarehouseModel DivisionWarehouse { get; set; } = new();

    private readonly int _divisionId;
    private readonly DivisionWarehouseProvider _divisionWarehouseProvider;
    private readonly GlobalDataProvider _globalDataProvider;


    public DivisionWarehousePage(DivisionWarehouseProvider divisionWarehouseProvider, GlobalDataProvider globalDataProvider, int divisionId)
	{
        _divisionId = divisionId;
        _divisionWarehouseProvider = divisionWarehouseProvider;
        _globalDataProvider = globalDataProvider;

        InitializeComponent();
        Loaded += OnLoadingView;
        UpdateModelCommand = new Command(OnUpdateModelCommand);

        BindingContext = this;
        ProductsView.BindingContext = DivisionWarehouse;
    }

    private async void OnUpdateModelCommand(object obj)
    {
        try
        {
            var result = await _divisionWarehouseProvider.UpdatedDivisionWarehouseAsync(DivisionWarehouse);
            if (!result.Success && !string.IsNullOrEmpty(result.Message))
            {
                await DisplayAlert("Error", result.Message, "OK");
            }
            else
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnLoadingView(object? sender, EventArgs e)
    {
        try
        {
            var warehouse = await _divisionWarehouseProvider.GetDivisionWarehouseAsync(_divisionId);
            DivisionWarehouse.DivisionWarehouseId = warehouse.DivisionWarehouseId;
            DivisionWarehouse.DivisionId = warehouse.DivisionId;
            DivisionWarehouse.WarehouseCapacity = warehouse.WarehouseCapacity;
            DivisionWarehouse.CurrentFilling = warehouse.CurrentFilling;

            DivisionWarehouse.Products.Clear();

            double totalOccupiedVolume = 0;
            foreach (var product in warehouse.Products)
            {
                var definition = _globalDataProvider.ProductDefinitions.FirstOrDefault(pf=>pf.ProductDefinitionId == product.ProductDefinitionId);
                if (definition != null)
                {
                    product.Name = definition.Name;
                    product.ProductIcon = definition.Image;
                    product.Volume = (definition.Volume * product.Quantity) / DivisionWarehouse.WarehouseCapacity;
                    totalOccupiedVolume += product.Volume;
                }
                DivisionWarehouse.Products.Add(product);
            }

            DivisionWarehouse.CurrentFilling = totalOccupiedVolume;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }
}