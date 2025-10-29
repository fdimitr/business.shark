using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using System.Collections.Frozen;

namespace BusinessSharkService.Handlers.Interfaces
{
    public interface IWorldContext
    {
        List<Country> Countries { get; set; }
        Dictionary<int, BaseDivision> Divisions { get; set; }
        FrozenDictionary<int, ProductDefinition> ProductDefinitions { get; set; }
        FinancialTransaction Transaction { get; set; }
        void FillDivisions();
    }
}
