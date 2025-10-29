using BusinessSharkService.DataAccess.Models.Divisions;

namespace BusinessSharkService.Handlers.Divisions
{
    public class WarehouseHandler
    {
        public required Warehouse WarehouseInput { get; set; }
        public required Warehouse WarehouseOutput { get; set; }

        private BaseDivision _baseDivision;

        public WarehouseHandler(BaseDivision baseDivision)
        { 
            _baseDivision = baseDivision;
            WarehouseInput = baseDivision.Warehouses.FirstOrDefault(w => w.Type == (int)WarehouseType.Input);
            WarehouseInput = baseDivision.Warehouses.FirstOrDefault(w => w.Type == (int)WarehouseType.Output);
        }
    }
}
