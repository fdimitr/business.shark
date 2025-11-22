using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Logic.Models;

namespace BusinessSharkClient.Logic
{
    public class GlobalDataProvider(ILocalRepository<ProductDefinitionEntity> pDRepo, ILocalRepository<ComponentUnitEntity> cRepo, ILocalRepository<ProductCategoryEntity> pCRepo)
    {
        public List<ProductDefinitionModel> ProductDefinitions { get; set; } = new();
        public List<ProductCategoryModel> ProductCategories { get; set; } = new();

        public async Task LoadData()
        {
            // Product Category
            var categories = await pCRepo.GetAllAsync();

            ProductCategories.Clear();
            foreach (var cat in categories)
            {
                var catModel = new ProductCategoryModel
                {
                    ProductCategoryId = cat.Id,
                    Name = cat.Name,
                    SortOrder = cat.SortOrder
                };
                ProductCategories.Add(catModel);
            }

            // Product Definition
            var definitions = await pDRepo.GetAllAsync();
            var components = await cRepo.GetAllAsync();

            ProductDefinitions.Clear();
            foreach (var def in definitions)
            {
                var defModel = new ProductDefinitionModel
                {
                    ProductDefinitionId = def.Id,
                    ProductCategoryId = def.ProductCategoryId,
                    Name = def.Name,

                    BaseProductionCount = def.BaseProductionCount,
                    DeliveryPrice = (decimal)def.DeliveryPrice,
                    Volume = def.Volume,

                    TechImpactQuality = def.TechImpactQuality,
                    ToolImpactQuality = def.ToolImpactQuality,
                    WorkerImpactQuality = def.WorkerImpactQuality,
                    TechImpactQuantity = def.TechImpactQuantity,

                    ToolImpactQuantity = def.ToolImpactQuantity,
                    WorkerImpactQuantity = def.WorkerImpactQuantity,
                    Image = ImageSource.FromStream(() => new MemoryStream(def.Image)),
                    ComponentUnits = components.Where(c => c.ProductDefinitionId == def.Id).Select(cuGrpc =>
                        new ComponentUnitModel
                        {
                            ProductDefinitionId = def.Id,
                            ComponentDefinitionId = cuGrpc.Id,
                            ProductionQuantity = cuGrpc.ProductionQuantity,
                            QualityImpact = cuGrpc.QualityImpact
                        }).ToList()
                };

                ProductDefinitions.Add(defModel);
            }
        }
    }
}
