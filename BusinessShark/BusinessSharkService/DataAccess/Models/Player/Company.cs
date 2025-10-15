using BusinessSharkService.DataAccess.Models.Divisions;

namespace BusinessSharkService.DataAccess.Models.Player
{
    public class Company
    {
        public int CompanyId { get; set; }
        public required string Name { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public List<Factory> Factories { get; set; } = new();
    }
}
