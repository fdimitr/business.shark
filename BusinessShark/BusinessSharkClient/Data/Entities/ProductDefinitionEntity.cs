using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class ProductDefinitionEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public int ProductCategoryId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

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
        public required byte[] Image { get; set; }
        public List<ComponentUnitEntity> ComponentUnits { get; set; } = new();

        public object[] GetKeyValues() => [Id];
    }
}
