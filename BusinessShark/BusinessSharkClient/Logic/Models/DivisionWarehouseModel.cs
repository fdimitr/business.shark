using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessSharkClient.Logic.Models
{
    public partial class DivisionWarehouseModel : ObservableObject
    {
        public int DivisionWarehouseId { get; set; }
        public int DivisionId { get; set; }

        [ObservableProperty] 
        private double warehouseCapacity;

        [ObservableProperty] 
        private double currentFilling;

        public ObservableCollection<WarehouseProductModel> Products { get; set; } = new();
    }
}
