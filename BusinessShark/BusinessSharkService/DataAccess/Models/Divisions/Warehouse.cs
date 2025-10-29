using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    [Comment("Standard Warehouse. Part of Division")]
    public class Warehouse
    {
        public int WarehouseId { get; set; }
        public int VolumeCapacity { get; set; }
        public int DivisionId { get; set; }
        public BaseDivision? BaseDivision { get; set; }
        public int Type { get; set; }
        public List<WarehouseProduct>? Products { get; set; } = new();
    }
}
