using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Player;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class PlayerHandler
    {
        private readonly DataContext _dbContext;
        public PlayerHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Player?> GetByLoginAsync(string login)
        {
            if (string.IsNullOrEmpty(login))
            { 
                throw new ArgumentException("Login cannot be null or empty", nameof(login));
            }

            return await _dbContext.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Login.ToLower() == login.ToLower());
        }
    }
}
