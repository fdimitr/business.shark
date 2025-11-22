using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class CityEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        public int CountryId { get; set; }
        public double Population { get; set; }
        public double AverageSalary { get; set; }
        public double BaseLandPrice { get; set; }
        public double LandTax { get; set; }
        public int WealthLevel { get; set; }
        public int Happiness { get; set; }

        public object[] GetKeyValues() => [Id];
    }
}
