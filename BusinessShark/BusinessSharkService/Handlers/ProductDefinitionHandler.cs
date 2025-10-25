using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class ProductDefinitionHandler
    {
        private  readonly DataContext _dbContext;
        public ProductDefinitionHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductDefinition>> PreloadProductDefinitionsAsync(uint timeStamp)
        {
            var productDefinitions = await _dbContext.ProductDefinitions.Include(p=>p.ComponentUnits).ToListAsync();
            return productDefinitions.Where(pd => pd.TimeStamp > timeStamp).ToList();
        }
    }
}
