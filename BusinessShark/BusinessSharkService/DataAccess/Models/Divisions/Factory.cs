using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    public class Factory : BaseDivision
    {
        public int ItemDefinitionId { get; set; }
        public ItemDefinition? ItemDefinition { get; set; }
        public double ProgressProduction { get; set; } // Percent of single product left on production
        public double ProgressQuality { get; set; }
        public double ProgressPrice { get; set; }
        public double TechLevel { get; set; }

        public int ToolsId { get; set; }
        public Tools? Tools { get; set; }

        public int WorkersId { get; set; }
        public Workers? Workers { get; set; }

        internal bool IsProductionCompleted { get; set; } = true;

    }
}
