using BusinessSharkService;
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
            var worldContextMock = new Mock<IWorldContext>();
            var factory = CreateFactoryWithResources(ProductDefinitions[(int)ProductType.Bed]);
            var factoryHandler = new FactoryHandler(worldContextMock.Object);
            // Act
            factoryHandler.StartCalculation(factory);

            // Assert
            factory.WarehouseOutput.TryGetItem((int)ProductType.Bed, out var item);
            Assert.That(item, Is.Not.Null);
            Assert.That(item.ProcessingQuantity, Is.EqualTo(1));
            Assert.That(item.ProcessingQuality, Is.EqualTo(2.6).Within(Tolerant));
        }

        [Test]
        public void StartCalculation_DoesNotProduce_WhenResourcesInsufficient()
        {
            // Arrange
            var factory = CreateFactoryWithResources(ProductDefinitions[(int)ProductType.Bed]);
            factory.WarehouseInput[(int)ProductType.Wood].Quantity = 0; // Insufficient wood

            var worldContextMock = new Mock<IWorldContext>();
            var factoryHandler = new FactoryHandler(worldContextMock.Object);

            // Act
            factoryHandler.StartCalculation(factory);
            
            // Assert
            Assert.That(factory.WarehouseOutput, Is.Empty);
        }

        [Test]
        public void StartCalculation_DoesNotProduce_WhenResourcesAbsent()
        {
            // Arrange
            var factory = CreateFactoryWithResources(ProductDefinitions[(int)ProductType.Bed]);
            factory.WarehouseInput.Clear(); // No resources

            var worldContextMock = new Mock<IWorldContext>();
            var factoryHandler = new FactoryHandler(worldContextMock.Object);

            // Act
            factoryHandler.StartCalculation(factory);

            // Assert
            Assert.That(factory.WarehouseOutput, Is.Empty);
        }

        [Test]
        public void StartCalculation_CompletesProduction_In_Few_Cycles()
        {
            // Arrange
            var productDef = (ProductDefinition)ProductDefinitions[(int)ProductType.Bed].Clone();
            productDef.BaseProductionCount = 0.5;
            var factory = CreateFactoryWithResources(productDef);

            var worldContextMock = new Mock<IWorldContext>();
            var factoryHandler = new FactoryHandler(worldContextMock.Object);

            // Act
            factoryHandler.StartCalculation(factory);
            factoryHandler.CompleteCalculation(factory);
            factoryHandler.StartCalculation(factory);
            factoryHandler.CompleteCalculation(factory);

            // Assert
            factory.WarehouseOutput.TryGetItem((int)ProductType.Bed, out var item);
            Assert.That(item, Is.Not.Null);
            Assert.That(item.ProcessingQuantity, Is.EqualTo(0));
            Assert.That(item.ProcessingQuality, Is.EqualTo(0));
            Assert.That(item.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void StartCalculation_OverProduction_TwoCycles_WithoutChangesInItems()
        {
            // Arrange
            var productDef = (ProductDefinition)ProductDefinitions[(int)ProductType.Bed].Clone();
            productDef.BaseProductionCount = 0.5;
            var factory = CreateFactoryWithResources(productDef);
            factory.Workers!.TechLevel = 3.0; // Increase worker tech level to boost production
            factory.Tools!.TechLevel = 3.0;   // Increase tool tech level to boost production

            var worldContextMock = new Mock<IWorldContext>();
            var factoryHandler = new FactoryHandler(worldContextMock.Object);

            // Act
            factoryHandler.StartCalculation(factory);
            factoryHandler.CompleteCalculation(factory);
            factoryHandler.StartCalculation(factory);
            factoryHandler.CompleteCalculation(factory);


            // Assert
            factory.WarehouseOutput.TryGetItem((int)ProductType.Bed, out var item);
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Quantity, Is.EqualTo(2));
            Assert.That(factory.ProgressProduction, Is.EqualTo(0.4).Within(Tolerance));
        }


        [Test]
        public void StartCalculation_OverProduction_TwoCycles_WithChangesInItems()
        {
            // Arrange
            var productDef = (ProductDefinition)ProductDefinitions[(int)ProductType.Bed].Clone();
            productDef.BaseProductionCount = 0.5;
            var factory = CreateFactoryWithResources(productDef);
            factory.Workers!.TechLevel = 3.0; // Increase worker tech level to boost production
            factory.Tools!.TechLevel = 3.0;   // Increase tool tech level to boost production

            var worldContextMock = new Mock<IWorldContext>();
            var factoryHandler = new FactoryHandler(worldContextMock.Object);


            // Act
            factory.WarehouseInput[(int)ProductType.Wood].Quality = 2.0f;
            factoryHandler.StartCalculation(factory);
            factoryHandler.CompleteCalculation(factory);

            var firstCycleQuality = factory.ProgressQuality;

            factory.WarehouseInput[(int)ProductType.Wood].Quality = 4.0f;
            factoryHandler.StartCalculation(factory);
            factoryHandler.CompleteCalculation(factory);

            var secondCycleQuality = factory.ProgressQuality;

            // Assert
            factory.WarehouseOutput.TryGetItem((int)ProductType.Bed, out var item);
            Assert.That(item, Is.Not.Null);
            Assert.That(firstCycleQuality, Is.LessThan(secondCycleQuality));
            Assert.That(item.Quality, Is.EqualTo((firstCycleQuality + secondCycleQuality) / 2).Within(Tolerance));
            Assert.That(factory.ProgressProduction, Is.EqualTo(0.4).Within(Tolerance));
        }

    }
}
