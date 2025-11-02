using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Handlers.Divisions;

namespace BusinessSharkService.Handlers
{
    public class CountryHandler
    {
        private readonly FactoryHandler _factoryHandler;
        private readonly DistributionCenterHandler _storageHandler;
        private readonly MineHandler _mineHandler;
        private readonly SawmillHandler _sawmillHandler;

        public CountryHandler(
            FactoryHandler factoryHandler,
            DistributionCenterHandler storageHandler,
            MineHandler mineHandler,
            SawmillHandler sawmillHandler)
        {
            _factoryHandler = factoryHandler;
            _storageHandler = storageHandler;
            _mineHandler = mineHandler;
            _sawmillHandler = sawmillHandler;
        }


        public void StartCalculation(CancellationToken stoppingToken, Country country)
        {
            foreach (var city in country.Cities)
            {
                if (stoppingToken.IsCancellationRequested) break;
                foreach (var factory in city.Factories)
                {
                    if (stoppingToken.IsCancellationRequested)
                    _factoryHandler.StartCalculation(factory);
                }

                foreach (var storage in city.DistributionCenters)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    _storageHandler.StartCalculation(storage);
                }
                foreach (var mine in city.Mines)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    _mineHandler.StartCalculation(mine);
                }
                foreach (var sawmill in city.Sawmills)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    _sawmillHandler.StartCalculation(sawmill);
                }
            }
        }

        public void CompleteCalculation(CancellationToken stoppingToken, Country country)
        {
            foreach (var city in country.Cities)
            {
                foreach (var factory in city.Factories)
                {
                    _factoryHandler.CompleteCalculation(factory);
                }
                foreach (var storage in city.DistributionCenters)
                {
                    _storageHandler.CompleteCalculation(storage);
                }
                foreach (var mine in city.Mines)
                {
                    _mineHandler.CompleteCalculation(mine);
                }
                foreach (var sawmill in city.Sawmills)
                {
                    _sawmillHandler.CompleteCalculation(sawmill);
                }
            }
        }
    }
}
