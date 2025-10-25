using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BusinessSharkService.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<ProductCategory> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ComponentUnit> ComponentUnits { get; set; }
        public virtual DbSet<ProductDefinition> ProductDefinitions { get; set; }
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
            if (Database != null) Database.Migrate();
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and keys if needed

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
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    // Configure relationships and keys if needed

        //    modelBuilder.Entity<ProductDefinition>()
        //        .Property(p => p.TimeStamp)
        //        .IsRowVersion()
        //        .IsConcurrencyToken()
        //        .HasColumnName("xmin")      // note: no quotes!
        //        .HasColumnType("xid")
        //        .ValueGeneratedOnAddOrUpdate();

        //    // Resources
        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Wood,
        //        Name = ProductType.Wood.GetDescription(),
        //        BaseProductionCount = 50,
        //        BaseProductionPrice = 5,
        //        DeliveryPrice = 0.6M,
        //        Volume = 3,
        //        IconPath = @"Resources\Products\Icons\icon_woods.png",
        //        ImagePath = @"Resources\Products\Images\woods.png"
        //    });
        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Leather,
        //        Name = ProductType.Leather.GetDescription(),
        //        BaseProductionCount = 8,
        //        BaseProductionPrice = 15,
        //        DeliveryPrice = 0.2M,
        //        Volume = 0.5,
        //        IconPath = @"Resources\Products\Icons\icon_leather.png",
        //        ImagePath = @"Resources\Products\Images\leather.png"
        //    });
        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Sofa,
        //        Name = ProductType.Sofa.GetDescription(),
        //        BaseProductionCount = 7,
        //        BaseProductionPrice = 110,
        //        DeliveryPrice = 3.0M,
        //        Volume = 3,

        //        TechImpactQuality = 0.2,
        //        ToolImpactQuality = 0.1,
        //        WorkerImpactQuality = 0.3,

        //        TechImpactQuantity = 0.1,
        //        ToolImpactQuantity = 0.2,
        //        WorkerImpactQuantity = 0.3,

        //        IconPath = @"Resources\Products\Icons\icon_sofa.png",
        //        ImagePath = @"Resources\Products\Images\sofa.png"
        //    });

        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Bed,
        //        Name = ProductType.Bed.GetDescription(),
        //        BaseProductionCount = 10,
        //        BaseProductionPrice = 60,
        //        DeliveryPrice = 2.0M,
        //        Volume = 3,

        //        TechImpactQuality = 0.3,
        //        ToolImpactQuality = 0.1,
        //        WorkerImpactQuality = 0.3,

        //        TechImpactQuantity = 0.1,
        //        ToolImpactQuantity = 0.2,
        //        WorkerImpactQuantity = 0.3,

        //        IconPath = @"Resources\Products\Icons\icon_bed.png",
        //        ImagePath = @"Resources\Products\Images\bed.png"
        //    });

        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Chair,
        //        Name = ProductType.Chair.GetDescription(),
        //        BaseProductionCount = 15,
        //        BaseProductionPrice = 35,
        //        DeliveryPrice = 1.0M,
        //        Volume = 1,

        //        TechImpactQuality = 0.3,
        //        ToolImpactQuality = 0.1,
        //        WorkerImpactQuality = 0.4,

        //        TechImpactQuantity = 0.1,
        //        ToolImpactQuantity = 0.2,
        //        WorkerImpactQuantity = 0.3,

        //        IconPath = @"Resources\Products\Icons\icon_chair.png",
        //        ImagePath = @"Resources\Products\Images\chair.png"
        //    });

        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Table,
        //        Name = ProductType.Table.GetDescription(),
        //        BaseProductionCount = 10,
        //        BaseProductionPrice = 45,
        //        DeliveryPrice = 1.5M,
        //        Volume = 1,

        //        TechImpactQuality = 0.2,
        //        ToolImpactQuality = 0.2,
        //        WorkerImpactQuality = 0.3,

        //        TechImpactQuantity = 0.1,
        //        ToolImpactQuantity = 0.2,
        //        WorkerImpactQuantity = 0.3,

        //        IconPath = @"Resources\Products\Icons\icon_table.png",
        //        ImagePath = @"Resources\Products\Images\table.png",                
        //    });

        //    modelBuilder.Entity<ProductDefinition>().HasData(new ProductDefinition
        //    {
        //        ProductDefinitionId = (int)ProductType.Clay,
        //        Name = ProductType.Clay.GetDescription(),
        //        BaseProductionCount = 30,
        //        BaseProductionPrice = 2,
        //        DeliveryPrice = 0.8M,
        //        Volume = 2
        //    });

        //    modelBuilder.Entity<ComponentUnit>().HasData(new ComponentUnit 
        //    { 
        //        ProductDefinitionId = (int)ProductType.Sofa, 
        //        ComponentDefinitionId = (int)ProductType.Wood, 
        //        ProductionQuantity = 5, 
        //        QualityImpact = 0.2 
        //    });
        //    modelBuilder.Entity<ComponentUnit>().HasData(new ComponentUnit 
        //    { 
        //        ProductDefinitionId = (int)ProductType.Sofa, 
        //        ComponentDefinitionId = (int)ProductType.Clay, 
        //        ProductionQuantity = 5, 
        //        QualityImpact = 0.2 
        //    });

        //    modelBuilder.Entity<ComponentUnit>().HasData(new ComponentUnit
        //    { 
        //        ProductDefinitionId = (int)ProductType.Bed, 
        //        ComponentDefinitionId = (int)ProductType.Wood, 
        //        ProductionQuantity = 4, 
        //        QualityImpact = 0.3 
        //    });

        //    modelBuilder.Entity<ComponentUnit>().HasData(new ComponentUnit
        //    {
        //        ProductDefinitionId = (int)ProductType.Chair,
        //        ComponentDefinitionId = (int)ProductType.Wood,
        //        ProductionQuantity = 2,
        //        QualityImpact = 0.2
        //    });

        //    modelBuilder.Entity<ComponentUnit>().HasData(new ComponentUnit
        //    {
        //        ProductDefinitionId = (int)ProductType.Table,
        //        ComponentDefinitionId = (int)ProductType.Wood,
        //        ProductionQuantity = 2,
        //        QualityImpact = 0.3
        //    });
        //}
    }
}
