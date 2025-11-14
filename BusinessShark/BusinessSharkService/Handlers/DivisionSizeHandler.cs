using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class DivisionSizeHandler
    {
        private readonly DataContext _dbContext;
        public DivisionSizeHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DivisionSize>> GetDivisionSizesAsync(int divisionTypeId)
        {
            return await _dbContext.DivisionSizes
                .Where(ds => ds.DivisionTypeId == divisionTypeId)
                .ToListAsync();
        }
    }
}
