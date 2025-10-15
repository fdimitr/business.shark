using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers;
using NUnit.Framework;

namespace BusinessSharkTests.Handlers
{
    public partial class FactoryHandlerTests
    {
        private Factory CreateFactory(
            double factoryTechLevel,
            double? toolTechLevel,
            double? workerTechLevel)
        {
            var factory = new Factory
            {
                TechLevel = factoryTechLevel,
                Name = string.Empty
            };

            if (toolTechLevel.HasValue)
            {
                factory.Tools = new Tools { TechLevel = toolTechLevel.Value };
            }

            if (workerTechLevel.HasValue)
            {
                factory.Workers = new Workers { TechLevel = workerTechLevel.Value };
            }

            return factory;
        }

        [Test]
        public void CalculateProductionQuantity_ReturnsZero_WhenProductDefinitionNull()
        {
            var factory = CreateFactory(
                factoryTechLevel: 2,
                toolTechLevel: 3,
                workerTechLevel: 4);
            var result = FactoryHandler.CalculateProductionQuantity(factory);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateProductionQuantity_ReturnsZero_WhenToolsNull()
        {
            var factory = CreateFactory(
                factoryTechLevel: 2,
                toolTechLevel: null,
                workerTechLevel: 4);
            var result = FactoryHandler.CalculateProductionQuantity(factory);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateProductionQuantity_ReturnsZero_WhenWorkersNull()
        {
            var factory = CreateFactory(
                factoryTechLevel: 2,
                toolTechLevel: 3,
                workerTechLevel: null);
            var result = FactoryHandler.CalculateProductionQuantity(factory);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateProductionQuantity_ComputesExpectedQuantity()
        {
            // Given
            var factory = CreateFactory(
                factoryTechLevel: 2,
                toolTechLevel: 3,
                workerTechLevel: 4);

            // When
            var result = FactoryHandler.CalculateProductionQuantity(factory);

            // Then
            // quantity = 2*1.5 + 3*2 + 4*0.5 = 3 + 6 + 2 = 11
            // total = 10 * 11 = 110
            Assert.That(result, Is.EqualTo(110).Within(1e-9));
        }

        [Test]
        public void CalculateProductionQuantity_ComputesZero_WhenImpactsZero()
        {
            var factory = CreateFactory(
                factoryTechLevel: 5,
                toolTechLevel: 7,
                workerTechLevel: 9);
            var result = FactoryHandler.CalculateProductionQuantity(factory);
            Assert.That(result, Is.EqualTo(0));
        }
    }
}
