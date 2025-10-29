using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Extensions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class SawmillHandler(IWorldContext worldContext) : BaseDivisionHandler<Sawmill>(worldContext)
    {
        public override void CalculateCosts(Sawmill sawmill)
        {
            if (sawmill is null) throw new ArgumentNullException(nameof(sawmill));

            double maintenanceCostsAmount = 0;
            if (sawmill.Tools != null)
            {
                maintenanceCostsAmount = sawmill.Tools.MaintenanceCostsAmount;
                sawmill.Tools.MaintenanceCostsAmount = 0; // Reset after accounting for costs
            }

            sawmill.SawmillTransactions = new SawmillTransactions
            {
                DivisionId = sawmill.DivisionId,
                TransactionDate = DateTime.UtcNow,
                SalesProductsAmount = 0.0,
                PlantingAmount = sawmill.PlantingCosts,
                EmployeeSalariesAmount = sawmill.Employees != null ? sawmill.Employees.SalaryPerEmployee * sawmill.Employees.TotalQuantity : 0,
                MaintenanceCostsAmount = maintenanceCostsAmount,
                RentalCostsAmount = sawmill.RentalCost,
                EmployeeTrainingAmount = 0.0
            };

            sawmill.PlantingCosts = 0.0; // Reset planting costs after accounting for them

        }

        /// <summary>
        /// Initiates the calculation process for a given sawmill, determining the production quality and quantity.
        /// </summary>
        /// <remarks>This method calculates the production quality and quantity based on the provided
        /// sawmill's current state. If the sawmill's product definition is <see langword="null"/>, the method returns
        /// without performing any calculations. The method adjusts the calculated quantity based on the sawmill's raw
        /// material reserves, ensuring the quantity is non-negative. If the adjusted quantity is greater than zero, it
        /// adds a new product to the sawmill's warehouse input.</remarks>
        /// <param name="sawmill">The sawmill for which the calculation is to be performed. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sawmill"/> is <see langword="null"/>.</exception>
        public override void StartCalculation(Sawmill sawmill)
        {
            if (sawmill is null) throw new ArgumentNullException(nameof(sawmill));
            if (sawmill.ProductDefinition is null || sawmill.WarehouseProductInput is null) return;

            // Calculate production quality and quantity
            var quality = CalculateProductionQuality(sawmill);
            var quantity = CalculateProductionQuantity(sawmill);

            // Adjust quantity based on raw material reserves, ensure non-negative
            var adjustedQuantity = quantity > sawmill.RawMaterialReserves ? Math.Max(0.0, quantity - sawmill.RawMaterialReserves) : quantity;
            if (adjustedQuantity <= 0.0) return;

            var addedQuantity = (int)Math.Round(adjustedQuantity);

            // Add to temporary warehouse to the end of calculation process
            sawmill.WarehouseProductInput.Add(new WarehouseProduct
            {
                WarehouseId = sawmill.InputWarehouse!.WarehouseId,
                ProductDefinitionId = sawmill.ProductDefinitionId,
                Quality = quality,
                Quantity = addedQuantity
            });
        }

        /// <summary>
        /// Finalizes the calculation for the sawmill division by transferring produced items
        /// from the input warehouse to the output warehouse, updating quantities and qualities.
        /// </summary>
        /// <param name="sawmill">The sawmill division to complete calculation for.</param>
        public override void CompleteCalculation(Sawmill sawmill)
        {
            if (sawmill is null) throw new ArgumentNullException(nameof(sawmill));
            if (sawmill.ProductDefinition is null 
                || sawmill.WarehouseProductInput is null
                || sawmill.WarehouseProductOutput is null)  return;

            // Try to get the product just produced in the input warehouse
            if (sawmill.WarehouseProductInput.TryGetItem(sawmill.ProductDefinitionId, out var addedItem))
            {
                // If the output warehouse already contains this product, update its quantity and quality
                if (sawmill.WarehouseProductOutput.TryGetItem(sawmill.ProductDefinitionId, out var item))
                {
                    // Calculate new average quality based on existing and added quantities/qualities
                    var newQuality = CalculateWarehouseQuality(item.Quantity, item.Quality, addedItem.Quantity, addedItem.Quality);
                    item.Quality = newQuality;
                    // Add all produced quantities from input warehouse to output warehouse
                    item.Quantity += sawmill.WarehouseProductInput.Sum(i => i.Quantity);
                }
                else
                {
                    // If product does not exist in output warehouse, add it as a new entry
                    var totalQuantity = addedItem.Quantity;
                    var averageQuality = addedItem.Quality;
                    sawmill.WarehouseProductOutput.Add(new WarehouseProduct
                    {
                        WarehouseId = sawmill.OutputWarehouse!.WarehouseId,
                        ProductDefinitionId = sawmill.ProductDefinitionId,
                        Quality = averageQuality,
                        Quantity = totalQuantity
                    });
                }

                // Recalculate costs after completing production
                CalculateCosts(sawmill);

                // Deduct used raw materials from reserves
                sawmill.RawMaterialReserves -= addedItem.Quantity;
                // Clear the input warehouse after transferring items
                sawmill.WarehouseProductInput.Clear();
            }
        }



        internal static double CalculateProductionQuality(Sawmill sawmill)
        {
            var itemDef = sawmill.ProductDefinition;
            if (itemDef is null || sawmill.Tools is null || sawmill.Employees is null)
            {
                return 0;
            }

            return sawmill.ResourceDepositQuality *
                (sawmill.TechLevel * itemDef.TechImpactQuality +
                sawmill.Tools!.TechLevel * itemDef.ToolImpactQuality +
                sawmill.Employees!.TechLevel * itemDef.WorkerImpactQuality);
        }

        internal static double CalculateProductionQuantity(Sawmill sawmill)
        {
            var itemDef = sawmill.ProductDefinition;
            if (itemDef is null || sawmill.Tools is null || sawmill.Employees is null)
            {
                return 0;
            }

            var quantity = sawmill.TechLevel * itemDef.TechImpactQuantity +
                sawmill.Tools.TechLevel * itemDef.ToolImpactQuantity +
                sawmill.Employees.TechLevel * itemDef.WorkerImpactQuantity;
            return itemDef.BaseProductionCount * quantity;
        }
    }
}
