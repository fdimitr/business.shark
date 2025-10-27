using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Player
{
    [Comment("They represent players in the game")]
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [Comment("User Name")]
        [Required]
        [MaxLength(30)]
        public required string Name { get; set; }

        [Comment("User Login")]
        [Required]
        [MaxLength(20)]
        public required string Login { get; set; }

        [Comment("Password hash")]
        [Required]
        public required string Password { get; set; }

        [Comment("Creation Date")]
        public DateOnly CreatedDate { get; set; }
    }
}
