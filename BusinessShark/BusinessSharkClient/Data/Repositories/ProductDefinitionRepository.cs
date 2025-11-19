using BusinessSharkClient.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Repositories
{
    public class ProductDefinitionRepository(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<List<ProductDefinitionEntity>> GetAllAsync() =>
            await _context.ProductDefinitions.OrderBy(p => p.Name).ToListAsync();

        public async Task DeleteAllAsync()
        {
            await _context.ProductDefinitions.ExecuteDeleteAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ProductDefinitionEntity> definitions)
        {
            await _context.ProductDefinitions.AddRangeAsync(definitions);
            await _context.SaveChangesAsync();
        }
    }
}
