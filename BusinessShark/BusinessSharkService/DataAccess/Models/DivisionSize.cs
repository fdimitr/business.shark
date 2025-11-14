using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessSharkService.DataAccess.Models
{
    public class DivisionSize
    {
        [Key]
        public int DivisionSizeId { get; set; }
        public int DivisionTypeId { get; set; }
        public int Size { get; set; }
        public double ConstructionCost { get; set; }

        public int MaxEmployeesQuantity { get; set; }
        public int MaxToolsQuantity { get; set; }

        public int WarehouseVolume { get; set; }
    }
}
