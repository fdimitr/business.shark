using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSharkService.DataAccess.Models.Items
{
    [Comment("Describes the production process of a product. What is produced and from what. Also, the influence of components on the product.")]
    [PrimaryKey(nameof(ProductDefinitionId), nameof(ComponentDefinitionId))]
    public class ComponentUnit
    {
        [Comment("The identifier of the product to be produced.")]
        public int ProductDefinitionId { get; set; }

        [Comment("The identifier of the product that is involved in the production process as a component.")]
        public int ComponentDefinitionId { get; set; }

        [Comment("The amount of component required to produce one unit of item")]
        public int ProductionQuantity { get; set; }

        [Comment("The impact of this component on the quality of the final product")]
        public double QualityImpact { get; set; }

        public ProductDefinition? ProductDefinition { get; set; }
    }
}
