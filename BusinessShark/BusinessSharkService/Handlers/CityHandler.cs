using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Handlers.Divisions;

namespace BusinessSharkService.Handlers
{
    public class CityHandler
    {
        private readonly DataContext _dataContext;

        public CityHandler(DataContext dataContext, DistributionCenterHandler storageHandler)
        {
            _dataContext = dataContext;
        }

        public async Task StartCalculation(CancellationToken stoppingToken, City city)
        {
            await Task.CompletedTask;
        }
    }
}
