using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class WarehouseProductsHandler
    {
        private readonly DataContext _dbContext;
        public WarehouseProductsHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<WarehouseProduct>> GetByWarehouseAsync(int warehouseId)
        {
            return await _dbContext.WarehouseProducts.AsNoTracking().Where(wp=>wp.WarehouseId == warehouseId).ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(int productWarehouseProductId, int productSalesLimit, double productSalesPrice, bool productAvailableForSale)
        {
            var product = await _dbContext.WarehouseProducts.FindAsync(productWarehouseProductId);
            if (product == null) return false;

            product.SalesLimit = productSalesLimit;
            product.SalesPrice = productSalesPrice;
            product.AvailableForSale = productAvailableForSale;

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
