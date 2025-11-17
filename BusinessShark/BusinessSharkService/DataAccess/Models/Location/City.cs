using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSharkService.DataAccess.Models.Location
{
    [Comment("Represent cities with properties and divisions list for each city")]
    public class City
    {
        [Key]
        public int CityId { get; set; }

        public required int CountryId { get; set; }
        public required Country Country { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        [Comment("The happiness level")]
        public int Happiness { get; set; } = 0;

        [Comment("The wealth level")]
        public int WealthLevel { get; set; } = 0;

        public int Population { get; set; } = 0;

        public double AverageSalary { get; set; } = 0;

        [Comment("Base Land price in the city")]
        public int BaseLandPrice { get; set; } = 0;

        [Comment("Land tax in the city")]
        public int LandTax { get; set; } = 0;

        public List<Division> Divisions { get; set; } = new();

        [NotMapped]
        public IReadOnlyList<Factory> Factories => Divisions.OfType<Factory>().ToList();

        [NotMapped]
        public IReadOnlyList<DistributionCenter> DistributionCenters => Divisions.OfType<DistributionCenter>().ToList();

        [NotMapped]
        public IReadOnlyList<Mine> Mines => Divisions.OfType<Mine>().ToList();

        [NotMapped]
        public IReadOnlyList<Sawmill> Sawmills => Divisions.OfType<Sawmill>().ToList();
    }
}
