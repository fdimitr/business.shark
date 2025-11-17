using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models
{
    public class World
    {
        [Key]
        public int Id { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
