using System.ComponentModel.DataAnnotations;
using BusinessSharkService.DataAccess.Models.Divisions;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Location
{
    [Comment("Represent cities with properties and divisions list for each city")]
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        [Comment("The happiness level")]
        public int Happiness { get; set; } = 0;

        [Comment("The wealth level")]
        public int WealthLevel { get; set; } = 0;

        [Comment("Base Land price in the city")]
        public int BaseLandPrice { get; set; } = 0;

        [Comment("Land tax in the city")]
        public int LandTax { get; set; } = 0;

        public List<Factory> Factories { get; set; } = new();
    }
}
