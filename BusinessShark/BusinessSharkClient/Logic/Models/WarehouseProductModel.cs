using CommunityToolkit.Mvvm.ComponentModel;
// ReSharper disable InconsistentNaming

namespace BusinessSharkClient.Logic.Models
{
    public partial class WarehouseProductModel : ObservableObject
    {
        public int WarehouseProductId { get; set; }
        public int ProductDefinitionId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        internal bool IsChanged;

        partial void OnSalesPriceChanged(double value)
        {
            IsChanged = true;
        }

        partial void OnSalesLimitChanged(int value)
        {
            IsChanged = true;
        }

        partial void OnAvailableForSaleChanged(bool value)
        {
            IsChanged = true;
        }
    }
}
