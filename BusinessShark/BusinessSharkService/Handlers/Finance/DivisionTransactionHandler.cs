using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Finance;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers.Finance
{
    public class DivisionTransactionHandler(DataContext dbContext)
    {
        public async Task<List<DivisionTransaction>> SyncAsync(int companyId, DateTime timestamp)
        {
            var query = from dt in dbContext.DivisionTransactions
                        join d in dbContext.Divisions on dt.DivisionId equals d.DivisionId
                        where d.CompanyId == companyId && dt.UpdatedAt > timestamp
                        orderby dt.TransactionDate descending
                        select dt;

            return await query.AsNoTracking()
                .Take(10)
                .OrderBy(c => c.TransactionDate)
                .ToListAsync();
        }
    }
}