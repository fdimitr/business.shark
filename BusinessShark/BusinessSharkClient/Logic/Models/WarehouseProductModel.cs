using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessSharkClient.Logic.Models
{
    public partial class WarehouseProductModel : ObservableObject
    {
        public int WarehouseProductId { get; set; }
        public int ProductDefinitionId { get; set; }

        [ObservableProperty] private string name;

        [ObservableProperty] private ImageSource productIcon;
        [ObservableProperty] private double volume;
        [ObservableProperty] private int quantity;
        [ObservableProperty] private double quality;
        [ObservableProperty] private double costPrice;
        [ObservableProperty] private bool availableForSale;
        [ObservableProperty] private double salesPrice;
        [ObservableProperty] private int salesLimit;
        [ObservableProperty] private double fillingPercent;

        internal bool isChanged;

        partial void OnSalesPriceChanged(double value)
        {
            isChanged = true;
        }

        partial void OnSalesLimitChanged(int value)
        {
            isChanged = true;
        }

        partial void OnAvailableForSaleChanged(bool value)
        {
            isChanged = true;
        }
    }
}
