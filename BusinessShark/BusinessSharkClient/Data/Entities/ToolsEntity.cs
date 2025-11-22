using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class ToolsEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        public int DivisionId { get; set; } 
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public double TechLevel { get; set; }
        public double Wear { get; set; }
        public double Efficiency { get; set; }
        public double MaintenanceCosts { get; set; }
        public int WarrantyDays { get; set; }
    }
}
