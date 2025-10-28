using BusinessSharkService;
using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Context;
using System.Collections.Frozen;

namespace BusinessSharkTests.Handlers
{
    internal class BaseDivisionHandlerTests : BaseHandlerTests
    {
        private Storage _fromDivision;
        private Storage _toDivision;
        private Product _fromProduct;
        private Product _toProduct;
        WorldContext _worldContext;

        [SetUp]
        public void SetUp()
        {
            var country = new Country
            {
                CountryId = 1,
                Name = "TestCountry"
            };

            var city = new City
            {
                CityId = 1,
                Name = "TestCity"
            };

            _fromDivision = new Storage { Name = "From Division" };
            _toDivision = new Storage { Name = "To Division" };

            _fromProduct = new Product
            {
                Quality = 10,
                Quantity = 100,
                ProductDefinitionId = ProductDefinitions[(int)ProductType.Wood].ProductDefinitionId
            };

            _toProduct = new Product
            {
                Quality = 5,
                Quantity = 50,
                ProductDefinitionId = ProductDefinitions[(int)ProductType.Wood].ProductDefinitionId
            };  

            _fromDivision.WarehouseOutput.Add(_fromProduct);
            _toDivision.WarehouseInput.Add(_toProduct);

            country.Cities.Add(city);
            city.Storages.Add(_fromDivision);
            city.Storages.Add(_toDivision);

            _worldContext = new WorldContext
            {
                ProductDefinitions = ProductDefinitions.ToFrozenDictionary(),
                Countries = new List<Country> { country }
            };
            _worldContext.FillDivisions();
        }

        [Test]
        public void StartTransferItems_TransfersCorrectQuantityAndQuality()
        {
            // Arrange
            var route = new DeliveryRoute
            {
                DivisionId = _fromDivision.DivisionId,
                ProductDefinitionId = (int)ProductType.Wood,
                DeliveryCount = 30
            };
            _toDivision.DeliveryRoutes.Add(route);

            StorageHandler storageHandler = new StorageHandler(_worldContext);

            // Act
            storageHandler.StartTransferItems(_toDivision);

            // Assert
            _toDivision.WarehouseInput.TryGetItem((int)ProductType.Wood, out var targetItem);
            Assert.That(targetItem.ProcessingQuantity, Is.EqualTo(30));

            _fromDivision.WarehouseOutput.TryGetItem((int)ProductType.Wood, out var outputItem);
            Assert.That(outputItem, Is.Not.Null);
            Assert.That(outputItem.Quantity, Is.EqualTo(70));
            Assert.That(targetItem.ProcessingQuality, Is.EqualTo(10));
        }

        [Test]
        public void StartTransferItems_TransfersAllIfNotEnough()
        {
            // Arrange
            var route = new DeliveryRoute
            {
                DivisionId = _fromDivision.DivisionId,
                ProductDefinitionId = (int)ProductType.Wood,
                DeliveryCount = 200
            };
            _toDivision.DeliveryRoutes.Add(route);

            StorageHandler storageHandler = new StorageHandler(_worldContext);

            // Act
            storageHandler.StartTransferItems(_toDivision);

            // Assert
            _toDivision.WarehouseInput.TryGetItem((int)ProductType.Wood, out var targetItem);
            Assert.That(targetItem.ProcessingQuantity, Is.EqualTo(100));

            _fromDivision.WarehouseOutput.TryGetItem((int)ProductType.Wood, out var outputItem);
            Assert.That(outputItem, Is.Not.Null);
            Assert.That(outputItem.Quantity, Is.EqualTo(0));
        }

        [Test]
        public void CompleteTransferItems_MovesProcessingToQuantityAndResets()
        {
            // Arrange
            // Arrange
            var route = new DeliveryRoute
            {
                DivisionId = _fromDivision.DivisionId,
                ProductDefinitionId = (int)ProductType.Wood,
                DeliveryCount = 30
            };
            _toDivision.DeliveryRoutes.Add(route);

            StorageHandler storageHandler = new StorageHandler(_worldContext);
            storageHandler.StartTransferItems(_toDivision);

            _toDivision.WarehouseInput.TryGetItem((int)ProductType.Wood, out var targetItem);
            int prevQuantity = targetItem.Quantity;

            // Act
            storageHandler.CompleteTransferItems(_toDivision);

            // Assert
            Assert.That(targetItem.Quantity, Is.EqualTo(prevQuantity + 30));
            Assert.That(targetItem.Quality, Is.EqualTo(6.875).Within(Tolerant));
            Assert.That(targetItem.ProcessingQuantity, Is.EqualTo(0));
            Assert.That(targetItem.ProcessingQuality, Is.EqualTo(0));
        }
    }
}
