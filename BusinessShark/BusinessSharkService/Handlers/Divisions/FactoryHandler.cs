using System.Diagnostics;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class FactoryHandler(IWorldContext worldContext) : DivisionHandler<Factory>(worldContext)
    {
        internal struct QualityItem(double quality, double qualityImpact)
        {
            public double Quality = quality;
            public double QualityImpact = qualityImpact;
        }

        public override void StartCalculation(Factory factory)
        {
            Debug.Assert(factory.ProductDefinition != null);
            if (factory.WarehouseProductOutput == null ||factory.WarehouseProductInput == null || factory.WarehouseProductInput.Count == 0)
            {
                return; // No product to produce or no resources available
            }

            double cycleProgressQuality = 0;

            if (factory.IsProductionCompleted)
            {
                // Start production
                if (PossibleToProduce(factory))
                {
                    factory.IsProductionCompleted = false;

                    // Take resources for production
                    var listForQualityCalc = new List<QualityItem>();
                    
                    foreach (var unit in factory.ProductDefinition.ComponentUnits)
                    {
                        factory.WarehouseProductInput.TryGetItem(unit.ComponentDefinitionId, out var item);
                        item.Quantity -= unit.ProductionQuantity;
                        listForQualityCalc.Add(new QualityItem(item.Quality, unit.QualityImpact));
                    }

                    // Calculate production quality
                    cycleProgressQuality = CalculateProductionQuality(factory, listForQualityCalc);
                }
                else
                {
                    return;
                }
            }

            var cycleProgressQuantity = CalculateProductionQuantity(factory);

            factory.ProgressQuality = CalculateWarehouseQuality(factory.ProgressProduction, factory.ProgressQuality, cycleProgressQuantity, cycleProgressQuality);
            factory.ProgressProduction += cycleProgressQuantity;


            // Completion of production
            if (factory.ProgressProduction >= 1)
            {
                var productionCount = (int)Math.Truncate(factory.ProgressProduction);
                factory.ProgressProduction -= productionCount;

                if (factory.WarehouseProductOutput.TryGetItem(factory.ProductDefinition.ProductDefinitionId, out WarehouseProduct storedItem))
                {
                    storedItem.ProcessingQuality = factory.ProgressQuality;
                    storedItem.ProcessingQuantity += productionCount;
                }
                else
                {
                    factory.WarehouseProductOutput.Add(new WarehouseProduct
                    {
                        ProductDefinitionId = factory.ProductDefinitionId,
                        ProductDefinition = factory.ProductDefinition,
                        ProcessingQuantity = productionCount,
                        ProcessingQuality = factory.ProgressQuality
                    });
                }

                factory.IsProductionCompleted = true;
                // Reset progress
                if (factory.ProgressProduction == 0) factory.ProgressQuality = 0;
            }

        }

        public override void CompleteCalculation(Factory factory)
        {
            if (factory.ProductDefinition != null && factory.WarehouseProductOutput.TryGetItem(factory.ProductDefinitionId, out var item))
            {
                if (item.ProcessingQuantity == 0)
                {
                    return; // No items to process
                }
                var newQuality = CalculateWarehouseQuality(item);

                item.Quantity += item.ProcessingQuantity;
                item.Quality = newQuality;

                item.ResetProcessing();
            }

        }

        private bool PossibleToProduce(Factory factory)
        {
            if (factory.ProductDefinition == null) return false;

            foreach (var unit in factory.ProductDefinition.ComponentUnits)
            {
                if (!factory.WarehouseProductInput.TryGetItem(unit.ComponentDefinitionId, out WarehouseProduct item) ||
                    item.Quantity < unit.ProductionQuantity)
                {
                    return false;
                }
            }

            return true;
        }

        internal static double CalculateProductionQuality(Factory factory, List<QualityItem> qualityItems)
        {
            var itemDef = factory.ProductDefinition;
            if (itemDef is null || factory.Tools is null || factory.Employees is null)
            {
                return 0;
            }

            return qualityItems.Sum(e => e.Quality * e.QualityImpact)
                   + factory.TechLevel * itemDef.TechImpactQuality
                   + factory.Tools!.TechLevel * itemDef.ToolImpactQuality
                   + factory.Employees!.TechLevel * itemDef.WorkerImpactQuality;
        }

        internal static double CalculateProductionQuantity(Factory factory)
        {
            var itemDef = factory.ProductDefinition;
            if (itemDef is null || factory.Tools is null || factory.Employees is null)
            {
                return 0;
            }

            var quantity = factory.TechLevel * itemDef.TechImpactQuantity
                           + factory.Tools.TechLevel * itemDef.ToolImpactQuantity
                           + factory.Employees.TechLevel * itemDef.WorkerImpactQuantity;
            return itemDef.BaseProductionCount * quantity;
        }

        public override void CalculateCosts(Factory Division)
        {
            throw new NotImplementedException();
        }
    }
}
