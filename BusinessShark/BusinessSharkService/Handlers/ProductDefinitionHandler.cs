using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class ProductDefinitionHandler(DataContext dbContext)
    {
        public async Task<List<ProductDefinition>> PreloadProductDefinitionsAsync(DateTime updatedAt)
        {
            var productDefinitions = await dbContext.ProductDefinitions
                .Where(pd => pd.UpdatedAt > updatedAt)
                .Include(p=>p.ComponentUnits).ToListAsync();
            return productDefinitions;
        }
    }
}
