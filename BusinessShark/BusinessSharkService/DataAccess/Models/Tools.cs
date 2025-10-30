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

        [Comment("Deprecation factor of the tools, ranging from 0 (completely deprecated) to 1 (new).")]
        public double Deprecation { get; set; }

        public double MaintenanceCostsAmount { get; set; }

        [NotMapped]
        public int ActiveQuantity => (int)Math.Round(TotalQuantity * Deprecation);
    }
}
