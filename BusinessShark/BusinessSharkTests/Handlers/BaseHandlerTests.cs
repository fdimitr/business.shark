using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkTests.Handlers
{
    public class BaseHandlerTests
    {
        protected Dictionary<ProductType, ProductDefinition> ProductDefinitions =
            new Dictionary<ProductType, ProductDefinition>
            {
                {
                    ProductType.Bed, new ProductDefinition
                    {
                        ProductDefinitionId = (int)ProductType.Bed,
                        Name = ProductType.Bed.ToString(),
                        Volume = 1.0f,
                        BaseProductionCount = 1,
                        TechImpactQuality = 1.0,
                        ToolImpactQuality = 1.0,
                        WorkerImpactQuality = 1.0,
                        TechImpactQuantity = 0.30,
                        ToolImpactQuantity = 0.5,
                        WorkerImpactQuantity = 0.2,
                        BaseProductionPrice = 0,
                        DeliveryPrice = new decimal(40.0),
                        ProductionUnits =
                        [
                            new ComponentUnit
                            {
                                ProductDefinitionId = (int)ProductType.Bed,
                                ComponentDefinitionId = (int)ProductType.Wood,
                                ProductionQuantity = 2,
                                QualityImpact = 1.0
                            },

                            new ComponentUnit
                            {
                                ProductDefinitionId = (int)ProductType.Bed,
                                ComponentDefinitionId = (int)ProductType.Leather,
                                ProductionQuantity = 1,
                                QualityImpact = 1.0f
                            }
                        ]
                    }
                },
                {
                    ProductType.Wood, new ProductDefinition
                    {
                        ProductDefinitionId = (int)ProductType.Wood,
                        Name = ProductType.Wood.ToString(),
                        Volume = 1.0f,
                        BaseProductionCount = 50,
                        TechImpactQuality = 1.0,
                        ToolImpactQuality = 1.0,
                        WorkerImpactQuality = 1.0,
                        TechImpactQuantity = 0.30,
                        ToolImpactQuantity = 0.5,
                        WorkerImpactQuantity = 0.2,
                        BaseProductionPrice = 0,
                        DeliveryPrice = new decimal(20.0)
                    }
                },
                {
                    ProductType.Leather, new ProductDefinition
                    {
                        ProductDefinitionId = (int)ProductType.Leather,
                        Name = ProductType.Leather.ToString(),
                        Volume = 1.0f,
                        BaseProductionCount = 20,
                        TechImpactQuality = 1.0,
                        ToolImpactQuality = 1.0,
                        WorkerImpactQuality = 1.0,
                        TechImpactQuantity = 0.30,
                        ToolImpactQuantity = 0.5,
                        WorkerImpactQuantity = 0.2,
                        BaseProductionPrice = 0,
                        DeliveryPrice = new decimal(10.0)
                    }
                }
            };


    }
}
