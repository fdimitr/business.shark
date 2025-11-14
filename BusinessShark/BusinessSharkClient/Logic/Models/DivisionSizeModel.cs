namespace BusinessSharkClient.Logic.Models
{
    public class DivisionSizeModel
    {
        public int DivisionSizeId { get; set; }
        public int Size { get; set; }
        public double ConstructionCost { get; set; }

        public int MaxEmployees { get; set; }
        public int MaxTools { get; set; }

        public int WarehouseVolume { get; set; }
    }
}
