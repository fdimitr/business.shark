using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers.Context;
using BusinessSharkService.Handlers.Divisions;
using BusinessSharkShared;
using System.Collections.Frozen;

namespace BusinessSharkTests.Handlers
{
    internal class DivisionHandlerTests : BaseHandlerTests
    {
        private DistributionCenter _fromDivision;
        private DistributionCenter _toDivision;
        private WarehouseProduct _fromProduct;
        private WarehouseProduct _toProduct;
        WorldContext _worldContext;

        [SetUp]
        public void SetUp()
        {
            var country = new Country
            {
                CountryId = 1,
                Code = "UA",
                Name = "TestCountry"
            };

            var city = new City
            {
                CityId = 1,
                CountryId = 1,
                Country = country,
                Name = "TestCity"
            };

            _fromDivision = new DistributionCenter
            {
                Name = "From Division",
                CityId = 1,
                Warehouses =
                [
                    new Warehouse
                    {
                        Type = (int)WarehouseType.Input
                    },

                    new Warehouse
                    {
                        Type = (int)WarehouseType.Output
                    }
                ]
            };
            _toDivision = new DistributionCenter
            {
                Name = "To Division",
                CityId = 1,
                Warehouses =
                [
                    new Warehouse
                    {
                        Type = (int)WarehouseType.Input
                    },

                    new Warehouse
                    {
                        Type = (int)WarehouseType.Output
                    }
                ]
            };

            _fromProduct = new WarehouseProduct
            {
                Quality = 10,
                Quantity = 100,
                ProductDefinitionId = ProductDefinitions[(int)ProductType.Wood].ProductDefinitionId
            };

            _toProduct = new WarehouseProduct
            {
                Quality = 5,
                Quantity = 50,
                ProductDefinitionId = ProductDefinitions[(int)ProductType.Wood].ProductDefinitionId
            };  

            _fromDivision.WarehouseProductOutput!.Add(_fromProduct);
            _toDivision.WarehouseProductInput!.Add(_toProduct);

            country.Cities.Add(city);
            city.Divisions.Add(_fromDivision);
            city.Divisions.Add(_toDivision);

            _worldContext = new WorldContext
            {
                ProductDefinitions = ProductDefinitions.ToFrozenDictionary(),
                Countries = [country]
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

            DistributionCenterHandler storageHandler = new DistributionCenterHandler(_worldContext);

            // Act
            storageHandler.StartTransferItems(_toDivision);

            // Assert
            _toDivision.WarehouseProductInput.TryGetItem((int)ProductType.Wood, out var targetItem);
            Assert.That(targetItem.ProcessingQuantity, Is.EqualTo(30));

            _fromDivision.WarehouseProductOutput.TryGetItem((int)ProductType.Wood, out var outputItem);
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

            DistributionCenterHandler storageHandler = new DistributionCenterHandler(_worldContext);

            // Act
            storageHandler.StartTransferItems(_toDivision);

            // Assert
            _toDivision.WarehouseProductInput.TryGetItem((int)ProductType.Wood, out var targetItem);
            Assert.That(targetItem.ProcessingQuantity, Is.EqualTo(100));

            _fromDivision.WarehouseProductOutput.TryGetItem((int)ProductType.Wood, out var outputItem);
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

            DistributionCenterHandler storageHandler = new DistributionCenterHandler(_worldContext);
            storageHandler.StartTransferItems(_toDivision);

            _toDivision.WarehouseProductInput.TryGetItem((int)ProductType.Wood, out var targetItem);
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
