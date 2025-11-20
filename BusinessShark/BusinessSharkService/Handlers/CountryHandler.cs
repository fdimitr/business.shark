using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Handlers.Divisions;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class CountryHandler(
        DataContext dataContext,
        FactoryHandler factoryHandler,
        DistributionCenterHandler storageHandler,
        MineHandler mineHandler,
        SawmillHandler sawmillHandler)
    {

        public async Task<List<Country>> GetCountriesAsync(DateTime updatedAt)
        {
            return await dataContext.Countries
                .Where(c => c.UpdatedAt > updatedAt)
                .ToListAsync();
        }

        public void StartCalculation(CancellationToken stoppingToken, Country country)
        {
            foreach (var city in country.Cities)
            {
                if (stoppingToken.IsCancellationRequested) break;
                foreach (var factory in city.Factories)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    factoryHandler.CalculationOfToolWear(factory);
                    factoryHandler.StartCalculation(factory);
                }

                foreach (var storage in city.DistributionCenters)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    storageHandler.CalculationOfToolWear(storage);
                    storageHandler.StartCalculation(storage);
                }
                foreach (var mine in city.Mines)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    mineHandler.CalculationOfToolWear(mine);
                    mineHandler.StartCalculation(mine);
                }
                foreach (var sawmill in city.Sawmills)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    sawmillHandler.CalculationOfToolWear(sawmill);
                    sawmillHandler.StartCalculation(sawmill);
                }
            }
        }

        public void CompleteCalculation(CancellationToken stoppingToken, Country country)
        {
            foreach (var city in country.Cities)
            {
                foreach (var factory in city.Factories)
                {
                    factoryHandler.CompleteCalculation(factory);
                }
                foreach (var storage in city.DistributionCenters)
                {
                    storageHandler.CompleteCalculation(storage);
                }
                foreach (var mine in city.Mines)
                {
                    mineHandler.CompleteCalculation(mine);
                }
                foreach (var sawmill in city.Sawmills)
                {
                    sawmillHandler.CompleteCalculation(sawmill);
                }
            }
        }
    }
}
