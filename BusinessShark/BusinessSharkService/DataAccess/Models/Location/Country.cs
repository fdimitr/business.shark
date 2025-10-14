using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Location
{
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
