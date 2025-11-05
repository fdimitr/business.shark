using BusinessSharkService.DataAccess;

namespace BusinessSharkService.Handlers.Finance
{
    public class DivisionTransactionHandler
    {
        private readonly DataContext _dbContext;
        public DivisionTransactionHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}