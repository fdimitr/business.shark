using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Handlers.Divisions;
using BusinessSharkService.Handlers.Interfaces;
using Moq;

namespace BusinessSharkTests.Handlers
{
    [TestFixture]
    public class SawmillHandlerTests
    {
        private Mock<IWorldContext> _mockWorldContext;
        private SawmillHandler _sawmillHandler;
        private Sawmill _sawmill;

        [SetUp]
        public void SetUp()
        {
            _mockWorldContext = new Mock<IWorldContext>();
            _sawmillHandler = new SawmillHandler(_mockWorldContext.Object);

            _sawmill = new Sawmill
            {
                Name = "Test Sawmill",
                DivisionId = 1,
                ProductDefinitionId = 101,
                ProductDefinition = new ProductDefinition
                {
                    Name = "Lumber",
                    ProductDefinitionId = 101,
                    BaseProductionCount = 50,
                    TechImpactQuality = 0.2,
                    ToolImpactQuality = 0.3,
                    WorkerImpactQuality = 0.5,
                    TechImpactQuantity = 0.3,
                    ToolImpactQuantity = 0.5,
                    WorkerImpactQuantity = 0.2
                },
                ResourceDepositQuality = 0.8,
                RawMaterialReserves = 100,
                TechLevel = 1.5,
                Tools = new Tools { TechLevel = 1.2 },
                Workers = new Employees { TechLevel = 1.1 },
                WarehouseInput = new List<WarehouseProduct>(),
                WarehouseOutput = new List<WarehouseProduct>()
            };
        }

        [Test]
        public void StartCalculation_ValidSawmill_AddsProductToWarehouseInput()
        {
            // Act
            _sawmillHandler.StartCalculation(_sawmill);

            // Assert
            Assert.That(_sawmill.WarehouseInput.Count, Is.EqualTo(1));
            var product = _sawmill.WarehouseInput.First();
            Assert.That(product.Quantity, Is.GreaterThan(0));
            Assert.That(product.Quality, Is.GreaterThan(0));
            Assert.That(product.DivisionId, Is.EqualTo(_sawmill.DivisionId));
            Assert.That(product.ProductDefinitionId, Is.EqualTo(_sawmill.ProductDefinitionId));
        }

        [Test]
        public void StartCalculation_NullSawmill_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => _sawmillHandler.StartCalculation(null), Throws.ArgumentNullException);
        }

        [Test]
        public void StartCalculation_NullProductDefinition_DoesNotAddToWarehouseInput()
        {
            // Arrange
            _sawmill.ProductDefinition = null;

            // Act
            _sawmillHandler.StartCalculation(_sawmill);

            // Assert
            Assert.That(_sawmill.WarehouseInput, Is.Empty);
        }

        [Test]
        public void CompleteCalculation_ValidSawmill_TransfersProductToWarehouseOutput()
        {
            // Arrange
            _sawmillHandler.StartCalculation(_sawmill);

            // Act
            _sawmillHandler.CompleteCalculation(_sawmill);

            // Assert
            Assert.That(_sawmill.WarehouseOutput.Count, Is.EqualTo(1));
            var product = _sawmill.WarehouseOutput.First();
            Assert.That(product.Quantity, Is.GreaterThan(0));
            Assert.That(product.Quality, Is.GreaterThan(0));
            Assert.That(product.DivisionId, Is.EqualTo(_sawmill.DivisionId));
            Assert.That(product.ProductDefinitionId, Is.EqualTo(_sawmill.ProductDefinitionId));
            Assert.That(_sawmill.WarehouseInput, Is.Empty);
        }

        [Test]
        public void CompleteCalculation_NoInputProduct_DoesNotModifyWarehouseOutput()
        {
            // Act
            _sawmillHandler.CompleteCalculation(_sawmill);

            // Assert
            Assert.That(_sawmill.WarehouseOutput, Is.Empty);
        }

        [Test]
        public void CompleteCalculation_DeductsRawMaterialReserves()
        {
            // Arrange
            _sawmillHandler.StartCalculation(_sawmill);
            var initialReserves = _sawmill.RawMaterialReserves;

            // Act
            _sawmillHandler.CompleteCalculation(_sawmill);

            // Assert
            Assert.That(_sawmill.RawMaterialReserves, Is.LessThan(initialReserves));
        }
    }
}