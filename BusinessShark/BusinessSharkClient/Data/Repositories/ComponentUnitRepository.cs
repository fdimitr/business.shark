using BusinessSharkClient.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Repositories
{
    public class ComponentUnitRepository(AppDbContext context)
    {
        public async Task<List<ComponentUnitEntity>> GetAllAsync() =>
            await context.ComponentUnits.ToListAsync();

        public async Task DeleteAllAsync()
        {
            await context.ComponentUnits.ExecuteDeleteAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ComponentUnitEntity> definitions)
        {
            await context.ComponentUnits.AddRangeAsync(definitions);
            await context.SaveChangesAsync();
        }
    }
}
