using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Finance;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers.Finance
{
    public class DivisionTransactionHandler
    {
        private readonly DataContext _dbContext;
        public DivisionTransactionHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DivisionTransaction>?> GetByDivisionAsync(int divisionId)
        {
            return await _dbContext.DivisionTransactions.AsNoTracking().Where(c => c.DivisionId == divisionId).ToListAsync();
        }
    }
}