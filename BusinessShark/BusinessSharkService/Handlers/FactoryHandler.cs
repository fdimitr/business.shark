using System.Diagnostics;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers
{
    public class FactoryHandler(IWorldContext worldHandler) : BaseDivisionHandler<Factory>(worldHandler)
    {
        internal struct QualityItem(double quality, double qualityImpact)
        {
            public double Quality = quality;
            public double QualityImpact = qualityImpact;
        }

        public override void StartCalculation(Factory factory)
        {
            Debug.Assert(factory.ProductDefinition != null);
            if (factory.WarehouseInput.Count == 0)
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
                        factory.WarehouseInput.TryGetItem(unit.ComponentDefinitionId, out var item);
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

                if (factory.WarehouseOutput.TryGetItem(factory.ProductDefinition.ProductDefinitionId, out Product storedItem))
                {
                    storedItem.ProcessingQuality = factory.ProgressQuality;
                    storedItem.ProcessingQuantity += productionCount;
                }
                else
                {
                    factory.WarehouseOutput.Add(new Product
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
            if (factory.ProductDefinition != null && factory.WarehouseOutput.TryGetItem(factory.ProductDefinitionId, out var item))
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
                if (!factory.WarehouseInput.TryGetItem(unit.ComponentDefinitionId, out Product item) ||
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
            if (itemDef is null || factory.Tools is null || factory.Workers is null)
            {
                return 0;
            }

            return qualityItems.Sum(e => e.Quality * e.QualityImpact)
                   + factory.TechLevel * itemDef.TechImpactQuality
                   + factory.Tools!.TechLevel * itemDef.ToolImpactQuality
                   + factory.Workers!.TechLevel * itemDef.WorkerImpactQuality;
        }

        internal static double CalculateProductionQuantity(Factory factory)
        {
            var itemDef = factory.ProductDefinition;
            if (itemDef is null || factory.Tools is null || factory.Workers is null)
            {
                return 0;
            }

            var quantity = factory.TechLevel * itemDef.TechImpactQuantity
                           + factory.Tools.TechLevel * itemDef.ToolImpactQuantity
                           + factory.Workers.TechLevel * itemDef.WorkerImpactQuantity;
            return itemDef.BaseProductionCount * quantity;
        }
    }
}
