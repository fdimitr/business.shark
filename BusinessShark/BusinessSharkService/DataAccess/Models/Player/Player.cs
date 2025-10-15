using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Player
{
    [Comment("They represent players in the game")]
    public class Player
    {
        public int PlayerId { get; set; }

        [Comment("User Name")]
        public required string Name { get; set; }

        [Comment("User Login")]
        public required string Login { get; set; }

        [Comment("Password hash")]
        public required string Password { get; set; }

        [Comment("Creation Date")]
        public DateOnly CreatedDate { get; set; }
    }
}
