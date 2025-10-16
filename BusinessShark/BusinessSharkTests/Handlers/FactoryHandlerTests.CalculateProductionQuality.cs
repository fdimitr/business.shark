using BusinessSharkService.DataAccess.Models;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Handlers;

namespace BusinessSharkTests.Handlers
{
    [TestFixture]
    public partial class FactoryHandlerTests
    {
        private Factory CreateFactory(
            ProductDefinition? def = null,
            int factoryTechLevel = 0,
            Tools? tools = null,
            Workers? workers = null)
        {
            return new Factory
            {
                ProductDefinition = def,
                TechLevel = factoryTechLevel,
                Tools = tools,
                Workers = workers,
                Name = string.Empty
            };
        }

        private Tools CreateTools(int techLevel) => new Tools { TechLevel = techLevel };
        private Workers CreateWorkers(int techLevel) => new Workers { TechLevel = techLevel };

        [Test]
        public void CalculateProductionQuality_ReturnsZero_WhenProductDefinitionIsNull()
        {
            var factory = CreateFactory(def: null, factoryTechLevel: 5, tools: CreateTools(3), workers: CreateWorkers(4));
            var qualityItems = new List<FactoryHandler.QualityItem> { new FactoryHandler.QualityItem { Quality = 10, QualityImpact = 0.2 } };

            var result = FactoryHandler.CalculateProductionQuality(factory, qualityItems);

            Assert.That(result, Is.EqualTo(0d));
        }

        [Test]
        public void CalculateProductionQuality_ReturnsZero_WhenToolsIsNull()
        {
            var def = ProductDefinitions[ProductType.Bed];
            var factory = CreateFactory(def: def, factoryTechLevel: 5, tools: null, workers: CreateWorkers(4));
            var qualityItems = new List<FactoryHandler.QualityItem> { new FactoryHandler.QualityItem { Quality = 10, QualityImpact = 0.2 } };

            var result = FactoryHandler.CalculateProductionQuality(factory, qualityItems);

            Assert.That(result, Is.EqualTo(0d));
        }

        [Test]
        public void CalculateProductionQuality_ReturnsZero_WhenWorkersIsNull()
        {
            var def = ProductDefinitions[ProductType.Bed];
            var factory = CreateFactory(def: def, factoryTechLevel: 5, tools: CreateTools(3), workers: null);
            var qualityItems = new List<FactoryHandler.QualityItem> { new FactoryHandler.QualityItem { Quality = 10, QualityImpact = 0.2 } };

            var result = FactoryHandler.CalculateProductionQuality(factory, qualityItems);

            Assert.That(result, Is.EqualTo(0d));
        }

        [Test]
        public void CalculateProductionQuality_ReturnsSum_WhenQualityItemsEmpty()
        {
            var def = ProductDefinitions[ProductType.Bed];
            def.TechImpactQuality = 1.5;
            def.ToolImpactQuality = 2.0;
            def.WorkerImpactQuality = 0.5;
            var factory = CreateFactory(def: def, factoryTechLevel: 2, tools: CreateTools(3), workers: CreateWorkers(4));
            var qualityItems = new List<FactoryHandler.QualityItem>(); // sum part = 0

            // Expected = 2*1.5 + 3*2.0 + 4*0.5 = 3 + 6 + 2 = 11
            var result = FactoryHandler.CalculateProductionQuality(factory, qualityItems);

            Assert.That(result, Is.EqualTo(11d).Within(1e-9));
        }

        [Test]
        public void CalculateProductionQuality_ComputesFullFormula_WithMultipleQualityItems()
        {
            var def = ProductDefinitions[ProductType.Bed];
            def.TechImpactQuality = 1.5;
            def.ToolImpactQuality = 2.0;
            def.WorkerImpactQuality = 0.5;
            var factory = CreateFactory(def: def, factoryTechLevel: 2, tools: CreateTools(3), workers: CreateWorkers(4));
            var qualityItems = new List<FactoryHandler.QualityItem>
                {
                    new FactoryHandler.QualityItem { Quality = 10, QualityImpact = 0.1 }, // 1.0
                    new FactoryHandler.QualityItem { Quality = 5,  QualityImpact = 0.2 }  // 1.0
                };

            // Quality items sum = 2
            // Factory tech contribution = 2 * 1.5 = 3
            // Tools contribution = 3 * 2.0 = 6
            // Workers contribution = 4 * 0.5 = 2
            // Total = 2 + 3 + 6 + 2 = 13
            var result = FactoryHandler.CalculateProductionQuality(factory, qualityItems);

            Assert.That(result, Is.EqualTo(13d).Within(1e-9));
        }
    }
}
