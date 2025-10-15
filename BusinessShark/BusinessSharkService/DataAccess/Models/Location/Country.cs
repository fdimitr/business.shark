using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Location
{
    [Comment("Represent countries with properties and list cities for each city")]
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        public List<City> Cities { get; set; } = new();
    }
}
