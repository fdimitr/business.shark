using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Frozen;

namespace BusinessSharkService.Handlers
{
    public class WorldHandler
    {
        private readonly CountryHandler _countryHandler;
        private readonly DataContext _dbContext;
        private readonly IWorldContext _worldContext;

        public WorldHandler(DataContext dbContext, IWorldContext worldContext, CountryHandler countryHandler)
        {
            _worldContext = worldContext;
            _dbContext = dbContext;
            _countryHandler = countryHandler;

            // Load All ItemDefinitions
            _worldContext.ProductDefinitions = dbContext.ProductDefinitions
                .AsNoTracking()
                .ToDictionary(i => i.ProductDefinitionId, i => i)
                .ToFrozenDictionary();

            // Load All Countries with related Cities
            _worldContext.Countries = dbContext.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .ToList();
        }

        public async Task LoadCalculationData()
        {
            foreach (var country in _worldContext.Countries)
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
            foreach (var country in _worldContext.Countries)
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
            _worldContext.FillDivisions();

            StartCalculation(stoppingToken);
            CompleteCalculation(stoppingToken);

            await SaveCalculationData();
        }

        public void StartCalculation(CancellationToken stoppingToken)
        {
            foreach (var country in _worldContext.Countries)
            {
                _countryHandler.StartCalculation(stoppingToken, country);
            }
        }

        public void CompleteCalculation(CancellationToken stoppingToken)
        {
            foreach (var country in _worldContext.Countries)
            {
                _countryHandler.CompleteCalculation(stoppingToken, country);
            }
        }
    }
}
