using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSharkClient.Data
{
    public static class DbInitializer
    {
        private const bool DEV_RESET_DB = false;

        public static void Initialize(AppDbContext db)
        {
            if (DEV_RESET_DB)
            {
                // Delete the existing database (if the schema changed)
                db.Database.EnsureDeleted();
            }

            // Create a new database (or do nothing if it already exists)
            // In the future, when you switch to migrations, replace EnsureCreated() with Migrate().
            db.Database.EnsureCreated();
        }
    }
}
