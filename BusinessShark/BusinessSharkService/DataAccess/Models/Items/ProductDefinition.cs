using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSharkService.DataAccess.Models.Items
{
    [Comment("They represent product definition and properties")]
    public class ProductDefinition : ICloneable
    {
        [Key]
        public int ProductDefinitionId { get; set; }

        public int ProductCategoryId { get; set; }

        public ProductCategory? Category { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        // Physical volume: keep as double unless you need decimal semantics.
        [Comment("Physical volume of one product unit, is used to calculate the amount of space occupied in the warehouse")]
        public double Volume { get; set; }

        [Comment("Components of production item")]
        public List<ComponentUnit> ComponentUnits { get; set; } = new();

        // Production counts might be fractional; double is safer for accumulation.
        [Comment("The basic quantity of production")]
        public double BaseProductionCount { get; set; }

        // Monetary values: use decimal.
        [Comment("The basic price of production")]
        public decimal BaseProductionPrice { get; set; }

        [Comment("The cost of delivery of a unit of this product per unit of distance")]
        public decimal DeliveryPrice { get; set; }

        // Coefficients impacting quality: double for precision.
        [Comment("Сoefficient of influence of technology on the quality of production of a given item")]
        public double TechImpactQuality { get; set; }

        [Comment("Сoefficient of influence of tools on the quality of production of a given item")]
        public double ToolImpactQuality { get; set; }

        [Comment("Сoefficient of influence of workers on the quality of production of a given item")]
        public double WorkerImpactQuality { get; set; }

        [Comment("Сoefficient of influence of technology on the quantity of production of a given item")]
        public double TechImpactQuantity { get; set; }

        [Comment("Сoefficient of influence of tools on the quantity of production of a given item")]
        public double ToolImpactQuantity { get; set; }

        [Comment("Сoefficient of influence of workers on the quantity of production of a given item")]
        public double WorkerImpactQuantity { get; set; }

        [StringLength(50)]
        public string? ImagePath { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Thresholds: monetary -> decimal, attractiveness coefficient -> double.
        //public double MinAttractivenessThreshold { get; set; } = 0d;
        //public decimal MaxPriceThreshold { get; set; } = decimal.MaxValue;

        // Demand coefficient
        public double Necessity { get; set; }

        public void CheckQualityTotalImpact()
        {
            var totalImpact = ComponentUnits.Sum(p => p.QualityImpact)
                              + TechImpactQuality
                              + ToolImpactQuality
                              + WorkerImpactQuality;
            if (Math.Abs(totalImpact - 1d) > 1e-9)
                throw new Exception($"Item: {Name}. The total coefficient of influence on quality should be equal to 1. But now it equals {totalImpact}");
        }

        public object Clone()
        {
            var clone = (ProductDefinition)this.MemberwiseClone();
            // Deep clone ProductionUnits
            clone.ComponentUnits = ComponentUnits
                .Select(unit => new ComponentUnit
                {
                    ProductDefinitionId = unit.ProductDefinitionId,
                    ComponentDefinitionId = unit.ComponentDefinitionId,
                    ProductionQuantity = unit.ProductionQuantity,
                    QualityImpact = unit.QualityImpact
                })
                .ToList();
            return clone;
        }
    }
}
