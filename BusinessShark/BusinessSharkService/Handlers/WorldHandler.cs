using BusinessSharkService.DataAccess;
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
                    city.BaseDivisions = await _dbContext.BaseDivisions.Where(s => s.CityId == city.CityId)
                        .Include(s=>s.WarehouseProductOutput)
                        .Include(s=>s.WarehouseProductInput)
                        .Include(s=>s.DeliveryRoutes)
                        .Include(f => f.Employees)
                        .Include(f => f.Tools)
                        .ToListAsync();
                }
            }
        }

        public async Task SaveCalculationData()
        {
            await _dbContext.SaveChangesAsync();
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
