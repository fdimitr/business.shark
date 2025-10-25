using BusinessSharkClient.Logic.Models;
using BusinessSharkService;

namespace BusinessSharkClient.Logic
{
    public class GlobalDataProvider
    {
        public List<ProductDefinitionModel> ProductDefinitions { get; set; } = new List<ProductDefinitionModel>();

        private ProductDefinitionService.ProductDefinitionServiceClient _productDefinitionClient;
        private Google.Protobuf.Collections.RepeatedField<ProductDefinitionGrpc>? _productDefinitionsGrpc = null;

        public GlobalDataProvider(ProductDefinitionService.ProductDefinitionServiceClient productDefinitionClient)
        {
            ; _productDefinitionClient = productDefinitionClient;
        }

        public async Task LoadData()
        {
            var response = await _productDefinitionClient.SyncAsync(new ProductDefinitionRequest { Timestamp = 0 });
            if (response == null) return;

            ProductDefinitions.Clear();
            foreach (var defGrpc in response.ProductDefinitions)
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
