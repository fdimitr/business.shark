using System.Collections.Frozen;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;

namespace BusinessSharkService.Handlers
{
    public class WorldHandler
    {
        public List<Country> Countries { get; set; } = new();

        public FrozenDictionary<int, ItemDefinition> ItemDefinitions { get; set; } =
            new Dictionary<int, ItemDefinition>().ToFrozenDictionary();

        public Dictionary<int, BaseDivision> Divisions { get; set; } = new Dictionary<int, BaseDivision>();

        /// <summary>
        /// Builds the Divisions frozen dictionary from all Countries -> Cities -> Factories.
        /// If duplicate DivisionId values exist, the first encountered factory is kept.
        /// </summary>
        public void FillDivisions()
        {
            var factories = Countries
                .SelectMany(c => c.Cities)
                .SelectMany(city => city.Factories);

            // Deduplicate by DivisionId (take first) then freeze.
            Divisions = factories
                .GroupBy(f => f.DivisionId)
                .Select(g => g.First())
                .ToDictionary(f => f.DivisionId, f => (BaseDivision)f);
        }
    }
}
