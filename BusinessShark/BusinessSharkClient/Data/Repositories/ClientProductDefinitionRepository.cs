using BusinessSharkClient.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Repositories
{
    public class ClientProductDefinitionRepository(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<List<ClientProductDefinition>> GetAll() =>
            await _context.ClientProductDefinitions.OrderBy(p => p.Name).ToListAsync();

        public async Task DeleteAll()
        {
            await _context.ClientProductDefinitions.ExecuteDeleteAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ClientProductDefinition> definitions)
        {
            await _context.ClientProductDefinitions.AddRangeAsync(definitions);
            await _context.SaveChangesAsync();
        }
    }
}
