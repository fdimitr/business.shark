using BusinessSharkClient.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ClientProductDefinition> ClientProductDefinitions => Set<ClientProductDefinition>();
        public DbSet<DataState> DataStates => Set<DataState>();
    }
}
