using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Location;

namespace BusinessSharkService.Handlers
{
    public class CityHandler
    {
        private readonly DataContext _dataContext;

        public CityHandler(DataContext dataContext, StorageHandler storageHandler)
        {
            _dataContext = dataContext;
        }

        public async Task StartCalculation(CancellationToken stoppingToken, City city)
        {
            foreach (var storage in city.Storages)
            {
                
            }

        }
    }
}
