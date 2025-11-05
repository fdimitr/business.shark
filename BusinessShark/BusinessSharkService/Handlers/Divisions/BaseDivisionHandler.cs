using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public abstract class DivisionHandler<T>(IWorldContext worldContext) where T: Division
    {
        protected IWorldContext WorldContext => worldContext;

        public abstract void StartCalculation(T Division);
        public abstract void CompleteCalculation(T Division);
        public abstract void CalculateCosts(T Division, int quantityProduced, double qualityProduced);


        public void StartTransferItems(T Division)
        {
            foreach (var route in Division.DeliveryRoutes)
            {
                var fromDivision = worldContext.Divisions[route.DivisionId];
                if (fromDivision.WarehouseProductOutput.TryGetItem(route.ProductDefinitionId, out var item))
                {
                    if (item is { Quantity: > 0 })
                    {
                        if (!Division.WarehouseProductInput.TryGetItem(route.ProductDefinitionId, out var targetItem))
                        {
                            targetItem = (WarehouseProduct)item.Clone();
                        }

                        fromDivision.WarehouseProductOutput.TryGetItem(route.ProductDefinitionId, out var sourceItem);

                        if (item.Quantity >= route.DeliveryCount)
                        {
                            targetItem.ProcessingQuality =
                                CalculateWarehouseQuality(targetItem.ProcessingQuantity, targetItem.ProcessingQuality, route.DeliveryCount, sourceItem.Quality);

                            targetItem.ProcessingQuantity += route.DeliveryCount;
                            sourceItem.Quantity -= route.DeliveryCount;
                        }
                        else
                        {
                            targetItem.ProcessingQuality =
                                CalculateWarehouseQuality(targetItem.ProcessingQuantity, targetItem.ProcessingQuality, sourceItem.Quantity, sourceItem.Quality);
                            targetItem.ProcessingQuantity += sourceItem.Quantity;

                            sourceItem.Quantity = 0;
                            sourceItem.Quality = 0;
                        }
                    }
                }

            }

        }

        public void CompleteTransferItems(T Division)
        {
            foreach (var route in Division.DeliveryRoutes)
            {
                if (Division.WarehouseProductInput.TryGetItem(route.ProductDefinitionId, out var item))
                {
                    if (item.ProcessingQuantity > 0)
                    {
                        var newQuality = CalculateWarehouseQuality(item);

                        item.Quantity += item.ProcessingQuantity;
                        item.Quality = newQuality;

                        item.ResetProcessing();
                    }
                }
            }
        }

        internal static double CalculateWarehouseQuality(double existingQuantity, double existingQuality, double addedQuantity, double addedQuality)
        {
            double totalWeight = existingQuantity + addedQuantity;
            if (totalWeight == 0)
                throw new InvalidOperationException("Суммарное количество не может быть нулевым.");

            double weightedSum = existingQuality * existingQuantity + addedQuality * addedQuantity;
            return weightedSum / totalWeight;
        }

        internal static double CalculateWarehouseQuality(WarehouseProduct item)
        {
            return CalculateWarehouseQuality(item.Quantity, item.Quality, item.ProcessingQuantity, item.ProcessingQuality);
        }

    }
}
