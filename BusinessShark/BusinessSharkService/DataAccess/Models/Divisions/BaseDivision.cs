using System.ComponentModel.DataAnnotations;
using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    public abstract class BaseDivision
    {
        [Key]
        public int DivisionId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public float RentalCost { get; set; }

        public List<Item> WarehouseInput = new();  //to
        public List<Item> WarehouseOutput = new(); //from

        public List<Route> Routes { get; set; } = new();


    }
}
