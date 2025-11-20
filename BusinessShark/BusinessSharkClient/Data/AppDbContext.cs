using BusinessSharkClient.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ProductCategoryEntity> Categories => Set<ProductCategoryEntity>();
        public DbSet<ComponentUnitEntity> ComponentUnits => Set<ComponentUnitEntity>();
        public DbSet<SawmillEntity> Sawmills => Set<SawmillEntity>();
        public DbSet<ProductDefinitionEntity> ProductDefinitions => Set<ProductDefinitionEntity>();
        public DbSet<DataState> DataStates => Set<DataState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDefinitionEntity>()
                .HasMany(pd => pd.ComponentUnits)
                .WithOne(cu => cu.ProductDefinition)
                .HasForeignKey(cu => cu.ProductDefinitionId);

        }
    }
}
