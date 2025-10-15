using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ComponentUnit> ProductionUnits { get; set; }
        public DbSet<ProductDefinition> ItemDefinitions { get; set; }
        public DbSet<Product> Items { get; set; }
        public DbSet<DeliveryRoute> DeliveryRoutes { get; set; }
        public DbSet<Tools> Tools { get; set; }
        public DbSet<Workers> Workers { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Mine> Mines { get; set; }
        public DbSet<Sawmill> Sawmills { get; set; }

        public DataContext() : base()
        {
            Database.Migrate();
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
