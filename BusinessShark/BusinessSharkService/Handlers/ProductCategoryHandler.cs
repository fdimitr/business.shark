using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class ProductCategoryHandler(DataContext dbContext)
    {
        public async Task<List<ProductCategory>> LoadAsync(DateTime timeStamp)
        {
            return await dbContext.Categories.Where(c=>c.UpdatedAt > timeStamp).AsNoTracking().ToListAsync();
        }
    }
}
