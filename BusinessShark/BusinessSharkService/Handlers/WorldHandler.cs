using System.Collections.Frozen;
using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class WorldHandler : IWorldHandler
    {
        public List<Country> Countries { get; set; }
        public FrozenDictionary<int, ProductDefinition> ProductDefinitions { get; set; }
        public Dictionary<int, BaseDivision> Divisions { get; set; } = new Dictionary<int, BaseDivision>();

        private readonly CountryHandler _countryHandler;
        private readonly DataContext _dbContext;

        public WorldHandler(DataContext dbContext, CountryHandler countryHandler)
        {
            _dbContext = dbContext;
            _countryHandler = countryHandler;

            // Load All ItemDefinitions
            ProductDefinitions = dbContext.ProductDefinitions
                .AsNoTracking()
                .ToDictionary(i => i.ProductDefinitionId, i => i)
                .ToFrozenDictionary();

            // Load All Countries with related Cities
            Countries = dbContext.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .ToList();
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

        public async Task LoadCalculationData()
        {
            foreach (var country in Countries)
            {
                foreach (var city in country.Cities)
                {
                    city.Storages = await _dbContext.Storages.Where(s => s.CityId == city.CityId)
                        .Include(s=>s.WarehouseOutput)
                        .Include(s=>s.WarehouseInput)
                        .Include(s=>s.DeliveryRoutes)
                        .Include(f => f.Workers)
                        .ToListAsync();
                    city.Factories = await _dbContext.Factories.Where(f => f.CityId == city.CityId)
                        .Include(f=> f.WarehouseOutput)
                        .Include(f=> f.WarehouseInput)
                        .Include(f=> f.DeliveryRoutes)
                        .Include(f=> f.Workers)
                        .Include(f=> f.Tools)
                        .ToListAsync();
                    city.Mines = await _dbContext.Mines.Where(m => m.CityId == city.CityId)
                        .Include(m =>m.WarehouseOutput)
                        .Include(m => m.WarehouseInput)
                        .Include(f => f.Workers)
                        .ToListAsync();
                    city.Sawmills = await _dbContext.Sawmills.Where(s => s.CityId == city.CityId)
                        .Include(s => s.WarehouseOutput)
                        .Include(s => s.WarehouseInput)
                        .Include(f => f.Workers)
                        .ToListAsync();
                }
            }
        }

        public async Task SaveCalculationData()
        {
            foreach (var country in Countries)
            {
                foreach (var city in country.Cities)
                {
                    //_dbContext.Storages.UpdateRange(city.Storages);
                    //_dbContext.Factories.UpdateRange(city.Factories);
                    //_dbContext.Mines.UpdateRange(city.Mines);
                    //_dbContext.Sawmills.UpdateRange(city.Sawmills);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task Calculate(CancellationToken stoppingToken)
        {
            await LoadCalculationData();
            FillDivisions();

            StartCalculation(stoppingToken);
            CompleteCalculation(stoppingToken);

            await SaveCalculationData();
        }

        public void StartCalculation(CancellationToken stoppingToken)
        {
            foreach (var country in Countries)
            {
                _countryHandler.StartCalculation(stoppingToken, country);
            }
        }

        public void CompleteCalculation(CancellationToken stoppingToken)
        {
            foreach (var country in Countries)
            {
                _countryHandler.CompleteCalculation(stoppingToken, country);
            }
        }
    }
}
