using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using NUnit.Framework;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace BusinessSharkTests.Handlers
{
    [TestFixture]
    public partial class FactoryHandlerTests : BaseHandlerTests
    {
        private Factory CreateFactoryWithResources(ProductDefinition productDef, float techLevel = 1.0f, float toolTechLevel = 1.0f, float workerTechLevel = 1.0f)
        {
            var tools = new Tools { TechLevel = toolTechLevel, TotalQuantity = 1 };
            var workers = new Workers { TechLevel = workerTechLevel, TotalQuantity = 1 };
            var factory = new Factory{
                DivisionId = 1,
                Name = "TestFactory",
                ProductDefinition = productDef,
                TechLevel = techLevel,
                Tools = tools,
                Workers = workers,
                WarehouseInput =
                {
                    // Add enough input resources
                    [(int)ProductType.Wood] = new Product
                    {
                        ProductId = 1,
                        ProductDefinition = productDef, 
                        Quantity = 10,
                        Quality = 1.0f
                    },
                    [(int)ProductType.Leather] = new Product
                    {
                        ProductId = 1,
                        ProductDefinition = productDef,
                        Quantity = 10,
                        Quality = 1.0f
                    }
                }
            };

            return factory;
        }


    }
}
