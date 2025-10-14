using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Items
{
    public class ProductionUnit
    {
        [Key]
        public int ProductDefinitionId { get; set; }
        [Key]
        public int ComponentDefinitionId { get; set; }

        [Description("The amount of component required to produce one unit of item")]
        public int ProductionQuantity { get; set; }

        [Description("The impact of this component on the quality of the final product")]
        public double QualityImpact { get; set; }
    }
}
