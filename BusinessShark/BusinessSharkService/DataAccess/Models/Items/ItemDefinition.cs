using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Items
{
    public class ItemDefinition
    {
        [Key]
        public int ItemDefinitionId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        // Physical volume: keep as double unless you need decimal semantics.
        [Description("Physical volume of one item unit, is used to calculate the amount of space occupied in the warehouse")]
        public double Volume { get; set; }

        [Description("Component of production item")]
        public List<ProductionUnit> ProductionUnits { get; set; } = new();

        // Production counts might be fractional; double is safer for accumulation.
        [Description("The basic quantity of production, provided that all the coefficients of influence are equal to 1")]
        public double BaseProductionCount { get; set; }

        // Monetary values: use decimal.
        [Description("The basic price of production, provided that all the coefficients of influence are equal to 1")]
        public decimal BaseProductionPrice { get; set; }
        
        public decimal DeliveryPrice { get; set; }

        // Coefficients impacting quality: double for precision.
        [Description("Сoefficient of influence of technology on the quality of production of a given item")]
        public double TechImpactQuality { get; set; }

        [Description("Сoefficient of influence of tools on the quality of production of a given item")]
        public double ToolImpactQuality { get; set; }

        [Description("Сoefficient of influence of workers on the quality of production of a given item")]
        public double WorkerImpactQuality { get; set; }

        [Description("Сoefficient of influence of technology on the quantity of production of a given item")]
        public double TechImpactQuantity { get; set; }

        [Description("Сoefficient of influence of tools on the quantity of production of a given item")]
        public double ToolImpactQuantity { get; set; }

        [Description("Сoefficient of influence of workers on the quantity of production of a given item")]
        public double WorkerImpactQuantity { get; set; }

        // Thresholds: monetary -> decimal, attractiveness coefficient -> double.
        public double MinAttractivenessThreshold { get; set; } = 0d;
        public decimal MaxPriceThreshold { get; set; } = decimal.MaxValue;

        // Demand coefficient
        public double Necessity { get; set; }

        public void CheckQualityTotalImpact()
        {
            var totalImpact = ProductionUnits.Sum(p => p.QualityImpact)
                              + TechImpactQuality
                              + ToolImpactQuality
                              + WorkerImpactQuality;
            if (Math.Abs(totalImpact - 1d) > 1e-9)
                throw new Exception($"Item: {Name}. The total coefficient of influence on quality should be equal to 1. But now it equals {totalImpact}");
        }
    }
}
