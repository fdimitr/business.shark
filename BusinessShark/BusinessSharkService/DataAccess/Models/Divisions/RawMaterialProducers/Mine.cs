using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers
{
    [Comment("Mine division producing raw materials from resource deposits")]
    public class Mine : Division
    {
        public int ProductDefinitionId { get; set; }
        public ProductDefinition? ProductDefinition { get; set; }

        public double ProgressProduction { get; set; } // percent of single product left on production
        public double ProgressQuality { get; set; }
        public double ProgressPrice { get; set; }
        public double ResourceDepositQuality { get; set; } = 5; // replace with the quality of this type of resource at the deposit
        public double TechLevel { get; set; } = 1; // technology level of the mine, affects quality and quantity of production
    }
}
