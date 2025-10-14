using System.ComponentModel.DataAnnotations;
using BusinessSharkService.DataAccess.Models.Divisions;

namespace BusinessSharkService.DataAccess.Models.Location
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        public int Happiness { get; set; } = 0;
        public int WealthLevel { get; set; } = 0;
        public int LandPrice { get; set; } = 0;
        public int LandTax { get; set; } = 0;

        public List<Factory> Factories { get; set; } = new();
    }
}
