using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Divisions;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers.Divisions
{
    public class WarehouseHandler
    {
        private readonly DataContext _dbContext;

        public WarehouseHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Warehouse?> GetByDivisionAsync(int divisionId, int type)
        {
            return await _dbContext.Warehouses.AsNoTracking().FirstOrDefaultAsync(c => c.DivisionId == divisionId && c.Type == type);
        }
    }
}
