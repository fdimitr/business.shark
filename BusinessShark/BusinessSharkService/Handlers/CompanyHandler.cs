using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Player;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class CompanyHandler
    {
        private readonly DataContext _dbContext;
        public CompanyHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Company?> GetByPlayer(int playerId)
        {
            return await _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c=>c.PlayerId == playerId);
        }
    }
}
