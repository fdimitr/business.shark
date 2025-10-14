using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models
{
    public class Route
    {
        [Key]
        public int RouteId { get; set; }
        public int DivisionId { get; set; }
        public int ItemDefinitionId { get; set; }
        public int TransferringCount { get; set; }

        public double DeliveryPrice { get; set; }

    }
}
