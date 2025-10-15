using BusinessSharkService.DataAccess.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    [Comment("Factory division, produces products from components")]
    public class Factory : BaseDivision
    {

        public int ProductDefinitionId { get; set; }
        public ProductDefinition? ProductDefinition { get; set; }

        [Comment("How many products is producing in current moment")]
        public double ProgressProduction { get; set; } // Percent of single product left on production

        [Comment("Current progression quantity")]
        public double ProgressQuality { get; set; }

        [Comment("Current progression price")]
        public double ProgressPrice { get; set; }

        [Comment("Technology level of the factory, affects quality and quantity of production")]
        public double TechLevel { get; set; }

        public int ToolsId { get; set; }
        public Tools? Tools { get; set; }

        public int WorkersId { get; set; }
        public Workers? Workers { get; set; }

        internal bool IsProductionCompleted { get; set; } = true;

    }
}
