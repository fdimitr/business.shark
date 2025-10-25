using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using System.Collections.Frozen;

namespace BusinessSharkService.Handlers.Interfaces
{
    public interface IWorldHandler
    {
        List<Country> Countries { get; set; }
        Dictionary<int, BaseDivision> Divisions { get; set; }
        FrozenDictionary<int, ProductDefinition> ProductDefinitions { get; set; }

        Task Calculate(CancellationToken stoppingToken);
        void FillDivisions();
    }
}