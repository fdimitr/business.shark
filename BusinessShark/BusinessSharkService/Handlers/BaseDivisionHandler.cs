using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Extensions;

namespace BusinessSharkService.Handlers
{
    public abstract class BaseDivisionHandler<T>(WorldHandler worldHandler) where T: BaseDivision
    {
        public abstract void StartCalculation(T baseDivision);
        public abstract void CompleteCalculation(T baseDivision);

        public void StartTransferItems(T baseDivision)
        {
            foreach (var route in baseDivision.Routes)
            {
                var fromDivision = worldHandler.Divisions[route.DivisionId];
                if (fromDivision.WarehouseOutput.TryGetItem(route.ItemDefinitionId, out var item))
                {
                    if (item is { Quantity: > 0 })
                    {
                        if (!baseDivision.WarehouseInput.TryGetItem(route.ItemDefinitionId, out var targetItem))
                        {
                            targetItem = (Item)item.Clone();
                        }

                        var sourceItem = fromDivision.WarehouseOutput[route.ItemDefinitionId];

                        if (item.Quantity >= route.TransferringCount)
                        {
                            targetItem.ProcessingQuality =
                                CalculateWarehouseQuality(targetItem.ProcessingQuantity, targetItem.ProcessingQuality, route.TransferringCount, sourceItem.Quality);

                            targetItem.ProcessingQuantity += route.TransferringCount;
                            sourceItem.Quantity -= route.TransferringCount;
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

        public void CompleteTransferItems(T baseDivision)
        {
            foreach (var route in baseDivision.Routes)
            {
                if (baseDivision.WarehouseInput.TryGetItem(route.ItemDefinitionId, out var item))
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

        internal static double CalculateWarehouseQuality(Item item)
        {
            return CalculateWarehouseQuality(item.Quantity, item.Quality, item.ProcessingQuantity, item.ProcessingQuality);
        }

    }
}
