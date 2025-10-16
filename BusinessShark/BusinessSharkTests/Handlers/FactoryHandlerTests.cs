using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Interfaces;
using Moq;

namespace BusinessSharkTests.Handlers
{
    [TestFixture]
    public partial class FactoryHandlerTests : BaseHandlerTests
    {
        private const float Tolerant = 0.0001f;

        private Factory CreateFactoryWithResources(ProductDefinition productDef, float techLevel = 1.0f, float toolTechLevel = 1.0f, float workerTechLevel = 1.0f)
        {
            var tools = new Tools { TechLevel = toolTechLevel, TotalQuantity = 1 };
            var workers = new Workers { TechLevel = workerTechLevel, TotalQuantity = 1 };
            var factory = new Factory{
                DivisionId = 1,
                Name = "TestFactory",
                ProductDefinitionId = productDef.ProductDefinitionId,
                ProductDefinition = productDef,
                TechLevel = techLevel,
                Tools = tools,
                Workers = workers,
            };

            foreach (var defintion in ProductDefinitions)
            {
                factory.WarehouseInput.Add(new Product
                {
                    ProductDefinitionId = (int)defintion.Key,
                    ProductDefinition = defintion.Value,
                    Quantity = 100,
                    Quality = 5
                });
            }

            return factory;
        }

        [Test]
        public void StartCalculation_ProducesItem_WhenResourcesAvailable()
        {
            // Arrange
            var worldHandlerMock = new Mock<IWorldHandler>();
            var factory = CreateFactoryWithResources(ProductDefinitions[ProductType.Bed]);
            var factoryHandler = new FactoryHandler(worldHandlerMock.Object);
            // Act
            factoryHandler.StartCalculation(factory);

            // Assert
            factory.WarehouseOutput.TryGetItem((int)ProductType.Bed, out var item);
            Assert.That(item, Is.Not.Null);
            Assert.That(item.ProcessingQuantity, Is.EqualTo(1));
            Assert.That(item.ProcessingQuality, Is.EqualTo(2.6).Within(Tolerant));
        }
    }
}
