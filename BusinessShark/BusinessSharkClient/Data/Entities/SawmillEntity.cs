using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class SawmillEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [StringLength(2)]

        public required string CountryCode { get; set; }

        [Required]
        [StringLength(30)]
        public required string City { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }
        public int ProductDefinitionId { get; set; }
        public int VolumeCapacity { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        public double ResourceDepositQuality { get; set; }
        public double RawMaterialReserves { get; set; }
        public double TechLevel { get; set; }
        public double PlantingCosts { get; set; }
        public double RentalCost { get; set; }
        public double QuantityBonus { get; set; }
        public double QualityBonus { get; set; }

        public object[] GetKeyValues() => [Id];
    }
}
