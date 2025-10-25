using BusinessSharkService;
using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkTests.Handlers
{
    public class BaseHandlerTests
    {
        protected const float Tolerant = 0.0001f;

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

                        TechImpactQuality = 0.3,
                        ToolImpactQuality = 0.1,
                        WorkerImpactQuality = 0.2,

                        TechImpactQuantity = 0.3,
                        ToolImpactQuantity = 0.5,
                        WorkerImpactQuantity = 0.2,

                        BaseProductionPrice = 0,
                        DeliveryPrice = new decimal(40.0),
                        ComponentUnits =
                        [
                            new ComponentUnit
                            {
                                ProductDefinitionId = (int)ProductType.Bed,
                                ComponentDefinitionId = (int)ProductType.Wood,
                                ProductionQuantity = 2,
                                QualityImpact = 0.2
                            },

                            new ComponentUnit
                            {
                                ProductDefinitionId = (int)ProductType.Bed,
                                ComponentDefinitionId = (int)ProductType.Leather,
                                ProductionQuantity = 1,
                                QualityImpact = 0.2
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
                        TechImpactQuality = 0.2,
                        ToolImpactQuality = 0.3,
                        WorkerImpactQuality = 0.5,
                        TechImpactQuantity = 0.3,
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
                        TechImpactQuality = 0.3,
                        ToolImpactQuality = 0.3,
                        WorkerImpactQuality = 0.4,
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
