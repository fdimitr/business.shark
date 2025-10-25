using BusinessSharkClient.Logic.Models;
using BusinessSharkService;

namespace BusinessSharkClient.Logic
{
    public class GlobalDataProvider
    {
        public List<ProductDefinitionModel> ProductDefinitions { get; set; } = new List<ProductDefinitionModel>();
        public List<ProductCategoryModel> ProductCategories { get; set; } = new List<ProductCategoryModel>();

        private ProductDefinitionService.ProductDefinitionServiceClient _productDefinitionClient;
        private ProductCategoryService.ProductCategoryServiceClient _productCategoryClient;

        public GlobalDataProvider(ProductDefinitionService.ProductDefinitionServiceClient productDefinitionClient,
            ProductCategoryService.ProductCategoryServiceClient productCategoryClient)
        {
            _productDefinitionClient = productDefinitionClient;
            _productCategoryClient = productCategoryClient;
        }

        public async Task LoadData()
        {
            // Product Category
            var responseCategory = await _productCategoryClient.LoadAsync(new Google.Protobuf.WellKnownTypes.Empty());
            if (responseCategory == null) return;

            ProductCategories.Clear();
            foreach (var catGrpc in responseCategory.ProductCategories)
            {
                var catModel = new ProductCategoryModel
                {
                    ProductCategoryId = catGrpc.ProductCategoryId,
                    Name = catGrpc.Name,
                    SortOrder = catGrpc.SortOrder
                };
                ProductCategories.Add(catModel);
            }

            // Product Definition
            var responseDefinition = await _productDefinitionClient.SyncAsync(new ProductDefinitionRequest { Timestamp = 0 });
            if (responseDefinition == null) return;

            ProductDefinitions.Clear();
            foreach (var defGrpc in responseDefinition.ProductDefinitions)
            {
                var defModel = new ProductDefinitionModel(defGrpc.ProductDefinitionId, defGrpc.Name)
                {
                    BaseProductionCount = defGrpc.BaseProductionCount,
                    DeliveryPrice = (decimal)defGrpc.DeliveryPrice,

                    TechImpactQuality = defGrpc.TechImpactQuality,
                    ToolImpactQuality = defGrpc.ToolImpactQuality,
                    WorkerImpactQuality = defGrpc.WorkerImpactQuality,
                    TechImpactQuantity = defGrpc.TechImpactQuantity,

                    ToolImpactQuantity = defGrpc.ToolImpactQuantity,
                    WorkerImpactQuantity = defGrpc.WorkerImpactQuantity,
                    Icon = ImageSource.FromStream(() => new MemoryStream(defGrpc.Icon.ToByteArray())),
                    Image = ImageSource.FromStream(() => new MemoryStream(defGrpc.Image.ToByteArray())),
                    ComponentUnits = defGrpc.ComponentUnits.Select(cuGrpc => new ComponentUnitModel
                    {
                        ComponentDefinitionId = cuGrpc.ComponentDefinitionId,
                        ProductionQuantity = cuGrpc.ProductionQuantity,
                        QualityImpact = cuGrpc.QualityImpact
                    }).ToList()
                };

                ProductDefinitions.Add(defModel);


            }
        }
    }
}
