using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers
{
    public class Sawmill : Division
    {
        public int ProductDefinitionId { get; set; }
        public ProductDefinition? ProductDefinition { get; set; }

        public double ResourceDepositQuality { get; set; } = 5; // replace with the quality of this type of resource at the deposit
        public double RawMaterialReserves { get; set; } // amount of raw material reserves left in the deposit
        public double TechLevel { get; set; } = 1; // technology level of the mine, affects quality and quantity of production
        public double PlantingCosts { get; set; }
    }
}
