using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Handlers.Divisions;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers.Location
{
    public class CityHandler
    {
        private readonly DataContext _dataContext;

        public CityHandler(DataContext dataContext, DistributionCenterHandler storageHandler)
        {
            _dataContext = dataContext;
        }

        public async Task<List<City>> LoadAsync(DateTime updatedAt)
        {
            return await _dataContext.Cities
                .Where(c => c.UpdatedAt > updatedAt)
                .ToListAsync();
        }

        public async Task StartCalculation(CancellationToken stoppingToken, City city)
        {
            await Task.CompletedTask;
        }
    }
}
