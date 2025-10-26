namespace BusinessSharkClient.Logic.Models
{
    public class ProductDefinitionModel
    {
        public int ProductDefinitionId { get; set; }
        public int ProductCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Volume { get; set; }
        public List<ComponentUnitModel> ComponentUnits { get; set; } = new();

        public double BaseProductionCount { get; set; }
        public decimal BaseProductionPrice { get; set; }
        public decimal DeliveryPrice { get; set; }

        public double TechImpactQuality { get; set; }
        public double ToolImpactQuality { get; set; }
        public double WorkerImpactQuality { get; set; }

        public double TechImpactQuantity { get; set; }
        public double ToolImpactQuantity { get; set; }
        public double WorkerImpactQuantity { get; set; }

        public ImageSource? Icon { get; set; }
        public ImageSource? Image { get; set; }

        public uint TimeStamp { get; set; }
    }
}
