using BusinessSharkClient.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Repositories
{
    public class DataStateRepository(IDbContextFactory<AppDbContext> dbFactory)
    {
        public async Task<DataState?> GetByIdAsync(string id)
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            return await db.DataStates.FindAsync(id);
        }

        public async Task UpsertAsync(DataState entity)
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var existing = await db.DataStates.FindAsync(entity.Key);
            if (existing == null)
            {
                db.DataStates.Add(entity);
            }
            else
            {
                existing.Value = entity.Value;
            }
            await db.SaveChangesAsync();
        }
    }
}
