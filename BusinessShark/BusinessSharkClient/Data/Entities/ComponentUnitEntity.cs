using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessSharkClient.Data.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Entities
{
    [PrimaryKey(nameof(ProductDefinitionId), nameof(Id))]
    public class ComponentUnitEntity : IEntity
    {
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public int ProductDefinitionId { get; set; }
        public ProductDefinitionEntity? ProductDefinition { get; set; }
        public double ProductionQuantity { get; set; }
        public double QualityImpact { get; set; }

        [NotMapped]
        public ImageSource? Image { get; set; }
        [NotMapped]
        public string? Name { get; set; }
    }
}
