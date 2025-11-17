using BusinessSharkService.DataAccess.Models.Divisions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSharkService.DataAccess.Models
{
    [Comment("Represents tools used in production, affecting quantity and quality based on technology level and deprecation.")]
    public class Tools
    {
        [Key]
        public int ToolsId { get; set; }

        public int DivisionId { get; set; }
        public Division? Division { get; set; }

        public int TotalQuantity { get; set; }

        [Comment("Technology level of the tools, influencing their effectiveness.")]
        public double TechLevel { get; set; } = 1;

        [Comment("Wear coefficient of the tools, ranging from 0 (completely worn out) to 1 (new).")]
        public double WearCoefficient { get; set; }

        [Comment("Maximum quantity of tools that can be used in the connected division.")]
        public int MaxQuantity { get; set; }

        [Comment("Maintenance costs associated with the tools.")]
        public double MaintenanceCostsAmount { get; set; }

        [Comment("warranty period in days during which the defect does not wear out")]
        public int WarrantyDays { get; set; }

        // Computed property for efficiency
        [NotMapped]
        public double Efficiency => CalculateEfficiency();

        private double CalculateEfficiency()
        {
            if (MaxQuantity <= 0) return 0;

            var quantityFactor = Math.Min((double)TotalQuantity / MaxQuantity, 1d); // shortage only
            var wearFactor = 1 - Clamp01(WearCoefficient); // ensure bounds
            return quantityFactor * wearFactor;
        }

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);
    }
}
