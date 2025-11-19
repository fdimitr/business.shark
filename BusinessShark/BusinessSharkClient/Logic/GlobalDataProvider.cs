using BusinessSharkClient.Data.Repositories;
using BusinessSharkClient.Logic.Models;
using BusinessSharkService;

namespace BusinessSharkClient.Logic
{
    public class GlobalDataProvider(ProductDefinitionRepository productDefinitionRepository)
    {
        public List<ProductDefinitionModel> ProductDefinitions { get; set; } = new List<ProductDefinitionModel>();
        public List<ProductCategoryModel> ProductCategories { get; set; } = new List<ProductCategoryModel>();

        public async Task LoadData()
        {
            // Product Category
            //var responseCategory = await _productCategoryClient.LoadAsync(new Google.Protobuf.WellKnownTypes.Empty());
            //if (responseCategory == null) return;

            //ProductCategories.Clear();
            //foreach (var catGrpc in responseCategory.ProductCategories)
            //{
            //    var catModel = new ProductCategoryModel
            //    {
            //        ProductCategoryId = catGrpc.ProductCategoryId,
            //        Name = catGrpc.Name,
            //        SortOrder = catGrpc.SortOrder
            //    };
            //    ProductCategories.Add(catModel);
            //}

            // Product Definition
            var definitions = await productDefinitionRepository.GetAllAsync();

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
                    ComponentUnits = def.ComponentUnits.Select(cuGrpc => new ComponentUnitModel
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
