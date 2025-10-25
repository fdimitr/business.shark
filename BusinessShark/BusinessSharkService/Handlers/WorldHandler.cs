using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Handlers.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Frozen;

namespace BusinessSharkService.Handlers
{
    public class WorldHandler : IWorldHandler
    {
        public List<Country> Countries { get; set; } = new();

        public FrozenDictionary<int, ProductDefinition> ProductDefinitions { get; set; }

        public Dictionary<int, BaseDivision> Divisions { get; set; } = new Dictionary<int, BaseDivision>();

        public WorldHandler(DataContext dbContext)
        {
            // Load All ItemDefinitions
            ProductDefinitions = dbContext.ProductDefinitions
                .AsNoTracking()
                .ToDictionary(i => i.ProductDefinitionId, i => i)
                .ToFrozenDictionary();
        }

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

            var divisions= factories
                .Cast<BaseDivision>()
                .Concat(storages.Cast<BaseDivision>());

            // Deduplicate by DivisionId (take first) then freeze.
            Divisions = divisions
                .GroupBy(f => f.DivisionId)
                .Select(g => g.First())
                .ToDictionary(f => f.DivisionId, f => (BaseDivision)f);
        }

        public async Task Calculate(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}
