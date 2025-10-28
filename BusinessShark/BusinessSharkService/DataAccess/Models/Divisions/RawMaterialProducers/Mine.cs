using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers
{
    public class Mine : BaseDivision
    {
        public int ProductDefinitionId; // resource being produced
        public ProductDefinition? ProductDefinition { get; set; } = null!; 
        public double ProgressProduction { get; set; } // percent of single product left on production
        public double ProgressQuality { get; set; }
        public double ProgressPrice { get; set; }
        public double ResourceDepositQuality { get; set; } = 5; // replace with the quality of this type of resource at the deposit
        public double TechLevel { get; set; } = 1; // technology level of the mine, affects quality and quantity of production

        public int ToolsId { get; set; }
        public Tools? ToolPark { get; set; }

        public int WorkersId { get; set; }
    }
}
