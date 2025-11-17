using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models
{
    [Comment("Represents a route for transferring items between divisions")]
    public class DeliveryRoute
    {
        [Key]
        public int RouteId { get; set; }

        [Comment("The division from which the product will be transferred (from where)")]
        public int DivisionId { get; set; }

        public int ProductDefinitionId { get; set; }

        public int DeliveryCount { get; set; }

        public double DeliveryPrice { get; set; }

    }
}
