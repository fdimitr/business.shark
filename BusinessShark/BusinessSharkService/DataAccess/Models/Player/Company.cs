using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSharkService.DataAccess.Models.Player
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public required string Name { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
        public double Balance { get; set; }

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

