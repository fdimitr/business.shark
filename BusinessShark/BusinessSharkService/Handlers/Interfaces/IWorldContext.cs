using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using System.Collections.Frozen;

namespace BusinessSharkService.Handlers.Interfaces
{
    public interface IWorldContext
    {
        DateTime CurrentDate { get; set; }
        List<Country> Countries { get; set; }
        Dictionary<int, Division> Divisions { get; set; }
        FrozenDictionary<int, ProductDefinition> ProductDefinitions { get; set; }
        void FillDivisions();
    }
}
