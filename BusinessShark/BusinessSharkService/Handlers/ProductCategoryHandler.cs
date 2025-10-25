using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class ProductCategoryHandler
    {
        private readonly DataContext _dbContext;
        public ProductCategoryHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductCategory>> LoadAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }
    }
}
