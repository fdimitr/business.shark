using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using BusinessSharkService.Handlers.Interfaces;
using System.Collections.Frozen;

namespace BusinessSharkService.Handlers.Context
{
    public class WorldContext : IWorldContext
    {
        public List<Country> Countries { get; set; } = new();
        public required FrozenDictionary<int, ProductDefinition> ProductDefinitions { get; set; }
        public Dictionary<int, BaseDivision> Divisions { get; set; } = new();
        public FinancialTransaction Transaction { get; set; } = new();

        /// <summary>
        /// Builds the Divisions frozen dictionary from all Countries -> Cities -> Factories.
        /// If duplicate DivisionId values exist, the first encountered factory is kept.
        /// </summary>
        public void FillDivisions()
        {
            var factories = Countries
                .SelectMany(c => c.Cities)
                .SelectMany(city => city.Factories);

            var storages = Countries
                .SelectMany(c => c.Cities)
                .SelectMany(city => city.Storages);

            var mines = Countries
                .SelectMany(c => c.Cities)
                .SelectMany(city => city.Mines);

            var sawmills = Countries
                .SelectMany(c => c.Cities)
                .SelectMany(city => city.Sawmills);

            var divisions = factories
                .Cast<BaseDivision>()
                .Concat(storages.Cast<BaseDivision>())
                .Concat(mines.Cast<BaseDivision>())
                .Concat(sawmills.Cast<BaseDivision>());

            // Deduplicate by DivisionId (take first) then freeze.
            Divisions = divisions
                .GroupBy(f => f.DivisionId)
                .Select(g => g.First())
                .ToDictionary(f => f.DivisionId, f => (BaseDivision)f);
        }
    }
}
