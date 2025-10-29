using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BusinessSharkService.DataAccess
{
    public sealed class DataContext : DbContext
    {
        public DbSet<BaseDivision> BaseDivisions { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<DistributionCenter> Storages { get; set; }
        public DbSet<Mine> Mines { get; set; }
        public DbSet<Sawmill> Sawmills { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<ProductCategory> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ComponentUnit> ComponentUnits { get; set; }
        public DbSet<ProductDefinition> ProductDefinitions { get; set; }
        public DbSet<WarehouseProduct> Items { get; set; }
        public DbSet<DeliveryRoute> DeliveryRoutes { get; set; }
        public DbSet<Tools> Tools { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }

        public DataContext()
        {
            Database.Migrate();
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and keys if needed

            modelBuilder.Entity<BaseDivision>().ToTable("BaseDivisions");
            modelBuilder.Entity<Factory>().ToTable("Factories");
            modelBuilder.Entity<DistributionCenter>().ToTable("DistributionCenters");
            modelBuilder.Entity<Mine>().ToTable("Mines");
            modelBuilder.Entity<Sawmill>().ToTable("Sawmills");

            modelBuilder.Entity<BaseDivision>()
                .HasOne(d=>d.City)
                .WithMany(c=>c.BaseDivisions)
                .HasForeignKey(d=>d.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseDivision>()
                .HasMany(f => f.Warehouses)
                .WithOne(w => w.BaseDivision)
                .HasForeignKey(w => w.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Products)
                .WithOne(p => p.Warehouse)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // **************** Seeding Admin Player and Company ****************

            modelBuilder.Entity<Player>().HasData(new Player
            {
                PlayerId = 1,
                Name = "Admin",
                Login = "admin",
                Password = PasswordHelper.HashPassword("12345"),
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 1,
                PlayerId = 1,
                Name = "Admin Enterprise",
                Balance = 1000000.0
            });

            modelBuilder.Entity<ProductDefinition>()
                .Property(p => p.TimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            // **************** Seeding Product Categories **************************
            modelBuilder.Entity<ProductCategory>().HasData(new ProductCategory
            {
                ProductCategoryId = 1,
                Name = "Raw Materials",
                SortOrder = 1
            });
            modelBuilder.Entity<ProductCategory>().HasData(new ProductCategory
            {
                ProductCategoryId = 2,
                Name = "Household Furniture",
                SortOrder = 2
            });
            modelBuilder.Entity<ProductCategory>().HasData(new ProductCategory
            {
                ProductCategoryId = 3,
                Name = "Tableware",
                SortOrder = 3
            });

            // **************** Seeding Product Definition from JSON ****************
            // читаем JSON из файла
            string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "DataAccess\\ProductDefinitions.json"));

            // десериализуем в динамический объект
            var data = JsonConvert.DeserializeObject<List<ProductDefinition>>(json);
            if (data == null || data.Count == 0) return;

            // добавляем данные в модель
            foreach (var productDefinition in data)
            {
                modelBuilder.Entity<ProductDefinition>().HasData(productDefinition);

                foreach (var componentUnit in productDefinition.ComponentUnits)
                {
                    componentUnit.ProductDefinitionId = productDefinition.ProductDefinitionId;
                    // Seeding Component Units
                    modelBuilder.Entity<ComponentUnit>().HasData(componentUnit);
                }

                productDefinition.ComponentUnits = new(); // чтобы избежать повторного добавления
            }


            modelBuilder.Entity<Country>().HasData(new Country
            {
                CountryId = 1,
                Name = "Ukraine",
            });

            modelBuilder.Entity<City>().HasData(new City
            {
                CityId = 1,
                Name = "Kharkiv",
                CountryId = 1,
                Population = 1500000,
                AverageSalary = 1300.0
            });
            modelBuilder.Entity<City>().HasData(new City
            {
                CityId = 2,
                Name = "Kyiv",
                CountryId = 1,
                Population = 3000000,
                AverageSalary = 1500.0
            });
            modelBuilder.Entity<City>().HasData(new City
            {
                CityId = 3,
                Name = "Lviv",
                CountryId = 1,
                Population = 800000,
                AverageSalary = 1200.0
            });

            modelBuilder.Entity<Sawmill>().HasData(new Sawmill
            {
                DivisionId = 1,
                CityId = 1,
                Name = "Admin Sawmill",
                TechLevel = 1,
                RawMaterialReserves = Int32.MaxValue,
                ProductDefinitionId = 1, // Assuming ProductDefinition with ID 1 exists
                ResourceDepositQuality = 1.0,
            });

            modelBuilder.Entity<Employees>().HasData(new Employees
            {
                DivisionId = 1,
                TechLevel = 2,
                SalaryPerEmployee = 500.0,
                TotalQuantity = 5,
            });

            modelBuilder.Entity<Tools>().HasData(new Tools
            {
                DivisionId = 1,
                TechLevel = 2,
                Deprecation = 0.01,
                TotalQuantity = 5,
            });
        }
    }
}
