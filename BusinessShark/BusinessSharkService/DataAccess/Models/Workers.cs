using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models
{
    public class Workers
    {
        [Key]
        public int WorkersId { get; set; }
        public int TotalQuantity { get; set; }
        public double TechLevel { get; set; } = 1;

    }
}
