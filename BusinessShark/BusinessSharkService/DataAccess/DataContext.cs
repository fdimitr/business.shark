using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models.Finance;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using BusinessSharkShared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BusinessSharkService.DataAccess
{
    public sealed class DataContext : DbContext
    {
        public DbSet<DivisionSize> DivisionSizes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<DistributionCenter> Storages { get; set; }
        public DbSet<Mine> Mines { get; set; }
        public DbSet<Sawmill> Sawmills { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ProductCategory> Categories { get; set; }
        public DbSet<ComponentUnit> ComponentUnits { get; set; }
        public DbSet<ProductDefinition> ProductDefinitions { get; set; }
        public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
        public DbSet<DeliveryRoute> DeliveryRoutes { get; set; }
        public DbSet<Tools> Tools { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<DivisionTransaction> DivisionTransactions { get; set; }
        public DbSet<World> Worlds { get; set; }

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
            modelBuilder.Entity<Division>().ToTable("Divisions");
            modelBuilder.Entity<Factory>().ToTable("Factories");
            modelBuilder.Entity<DistributionCenter>().ToTable("DistributionCenters");
            modelBuilder.Entity<Mine>().ToTable("Mines");
            modelBuilder.Entity<Sawmill>().ToTable("Sawmills");
            modelBuilder.Entity<ProductDefinition>().ToTable("ProductDefinitions");
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Division>()
                .HasOne(d => d.City)
                .WithMany(c => c.Divisions)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Division>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Divisions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Division>()
                .HasMany(f => f.Warehouses)
                .WithOne(w => w.Division)
                .HasForeignKey(w => w.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Products)
                .WithOne(p => p.Warehouse)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductDefinition>()
                .Property(p => p.TimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<World>()

                .HasData(new World
            {
                Id = 1,
                CurrentDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

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
            {
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
                // *****************************************************************************


                modelBuilder.Entity<Country>().HasData(new Country
                {
                    CountryId = 1,
                    Code = "UA",
                    Name = "Ukraine",
                });

                modelBuilder.Entity<Country>().HasData(new Country
                {
                    CountryId = 2,
                    Code = "PL",
                    Name = "Poland",
                });

                modelBuilder.Entity<City>().HasData(new City
                {
                    CityId = 1,
                    Name = "Kharkiv",
                    CountryId = 1,
                    Country = null!,
                    Population = 1500000,
                    AverageSalary = 1300.0
                });
                modelBuilder.Entity<City>().HasData(new City
                {
                    CityId = 2,
                    Name = "Kyiv",
                    CountryId = 1,
                    Country = null!,
                    Population = 3000000,
                    AverageSalary = 1500.0
                });
                modelBuilder.Entity<City>().HasData(new City
                {
                    CityId = 3,
                    Name = "Lviv",
                    CountryId = 1,
                    Country = null!,
                    Population = 800000,
                    AverageSalary = 1200.0
                });
                modelBuilder.Entity<City>().HasData(new City
                {
                    CityId = 4,
                    Name = "Wroclaw",
                    CountryId = 2,
                    Country = null!,
                    Population = 800000,
                    AverageSalary = 2000.0
                });

                modelBuilder.Entity<DivisionSize>().HasData(
                    new DivisionSize
                    {
                        DivisionSizeId = 1, 
                        DivisionTypeId = (int)DivisionType.Sawmill, 
                        Size = 10,
                        WarehouseVolume = 1000,
                        ConstructionCost = 700000, 
                        MaxEmployeesQuantity = 6, 
                        MaxToolsQuantity = 3
                    },
                    new DivisionSize
                    {
                        DivisionSizeId = 2, 
                        DivisionTypeId = (int)DivisionType.Sawmill, 
                        Size = 50,
                        WarehouseVolume = 5000,
                        ConstructionCost = 3500000, 
                        MaxEmployeesQuantity = 10, 
                        MaxToolsQuantity = 5
                    },
                    new DivisionSize
                    {
                        DivisionSizeId = 3, 
                        DivisionTypeId = (int)DivisionType.Sawmill, 
                        Size = 100,
                        WarehouseVolume = 10000,
                        ConstructionCost = 7000000, 
                        MaxEmployeesQuantity = 20, 
                        MaxToolsQuantity = 10
                    },
                    new DivisionSize
                    {
                        DivisionSizeId = 4, 
                        DivisionTypeId = (int)DivisionType.Sawmill, 
                        Size = 500,
                        WarehouseVolume = 50000,
                        ConstructionCost = 35000000,
                        MaxEmployeesQuantity = 100, 
                        MaxToolsQuantity = 50
                    },
                    new DivisionSize
                    {
                        DivisionSizeId = 5, 
                        DivisionTypeId = (int)DivisionType.Sawmill,
                        Size = 1000,
                        WarehouseVolume = 10000,
                        ConstructionCost = 70000000,
                        MaxEmployeesQuantity = 200,
                        MaxToolsQuantity = 100
                    },
                    new DivisionSize
                    {
                        DivisionSizeId = 6,
                        DivisionTypeId = (int)DivisionType.Sawmill,
                        Size = 2000,
                        WarehouseVolume = 200000,
                        ConstructionCost = 140000000,
                        MaxEmployeesQuantity = 400,
                        MaxToolsQuantity = 200
                    }
                );

                // **************** Seeding Test Data (Admin Company) from JSON ****************
                string filePath = Path.Combine(AppContext.BaseDirectory, "DataAccess\\AdminSeeds.json");
                var adminSeedData = AdminSeedLoader.LoadAdminSeedData(filePath);

                // Access the deserialized objects
                List<Player> players = adminSeedData.Players ?? new List<Player>();
                List<Company> companies = adminSeedData.Companies ?? new List<Company>();
                List<Sawmill> sawmills = adminSeedData.Sawmills ?? new List<Sawmill>();
                List<Warehouse> warehouses = adminSeedData.Warehouses ?? new List<Warehouse>();
                List<Tools> tools = adminSeedData.Tools ?? new List<Tools>();
                List<Employees> employees = adminSeedData.Employees ?? new List<Employees>();

                foreach (var player in players)
                {
                    modelBuilder.Entity<Player>().HasData(player);
                }
                foreach (var company in companies)
                {
                    modelBuilder.Entity<Company>().HasData(company);
                }
                foreach (var sawmill in sawmills)
                {
                    modelBuilder.Entity<Sawmill>().HasData(sawmill);
                }
                foreach (var warehouse in warehouses)
                {
                    modelBuilder.Entity<Warehouse>().HasData(warehouse);
                }
                foreach (var tool in tools)
                {
                    modelBuilder.Entity<Tools>().HasData(tool);
                }
                foreach (var employee in employees)
                {
                    modelBuilder.Entity<Employees>().HasData(employee);
                }
                // *****************************************************************************
            }
        }
    }
}
