using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class ClientProductDefinition : IEntity
    {
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public double Volume { get; set; }
        public double BaseProductionCount { get; set; }
        public double BaseProductionPrice { get; set; }
        public double TechImpactQuality { get; set; }
        public double ToolImpactQuality { get; set; }
        public double WorkerImpactQuality { get; set; }
        public double TechImpactQuantity { get; set; }
        public double ToolImpactQuantity { get; set; }
        public double WorkerImpactQuantity { get; set; }
        public double DeliveryPrice { get; set; }
        public uint TimeStamp { get; set; }
        public byte[] Image { get; set; }
    }
}
