using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;

namespace BusinessSharkService.DataAccess.Models.Player
{
    public class Company
    {
        public int CompanyId { get; set; }
        public required string Name { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public List<Factory> Factories { get; set; } = new();
        public List<Storage> Storages { get; set; } = new();
        public List<Mine> Mines { get; set; } = new();
        public List<Sawmill> Sawmills { get; set; } = new();

        public double Balance { get; set; }
    }
}

