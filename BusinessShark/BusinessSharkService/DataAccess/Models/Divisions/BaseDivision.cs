using System.ComponentModel.DataAnnotations;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    public abstract class BaseDivision
    {
        [Key]
        public int DivisionId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [Comment("The volume capacity of the division warehouse")]
        public int VolumeCapacity { get; set; }

        [Comment("The cost of renting this division per month. Based on the city metric and city location")]
        public float RentalCost { get; set; }

        [Comment("Warehouses for the division")]
        public List<Warehouse> Warehouses = new();  //to

        public List<DeliveryRoute> DeliveryRoutes { get; set; } = new();

        public Tools? Tools { get; set; }

        public Employees? Employees { get; set; }

    }
}
