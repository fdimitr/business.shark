using BusinessSharkClient.Data.Entities.Interfaces;
using BusinessSharkClient.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Repositories
{
    public class EfLocalRepository<T>(IDbContextFactory<AppDbContext> dbFactory) : ILocalRepository<T>
        where T : class, IEntity
    {
        public async Task<List<T>> GetAllAsync()
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var set = db.Set<T>();
            var result = await set.AsNoTracking().OrderBy(x => x.Id).ToListAsync();

            return result;
        }

        public async Task<List<T>> GetDirtyAsync(int batchSize = 100)
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var set = db.Set<T>();
            return await set.Where(x => x.IsDirty)
                .OrderBy(x => x.Id)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task UpsertAsync(T entity, CancellationToken token)
        {
            // Use the write wrapper which acquires global write lock.
            await DbWriteExtensions.WriteAsync(async () =>
            {
                await using var db = await dbFactory.CreateDbContextAsync(token);
                var set = db.Set<T>();

                var exists = await set.FindAsync(entity.GetKeyValues(), token);
                if (exists == null)
                    await set.AddAsync(entity, token);
                else
                    db.Entry(exists).CurrentValues.SetValues(entity);

                await db.SaveChangesAsync(token);
            }, token);
        }

        public async Task UpsertRangeAsync(IEnumerable<T> entities, CancellationToken token)
        {
            int countInsert = 0;
            int countUpdated = 0;
            await DbWriteExtensions.WriteAsync(async () =>
            {
                await using var db = await dbFactory.CreateDbContextAsync(token);
                var set = db.Set<T>();

                foreach (var e in entities)
                {
                    var exists = await set.FindAsync(e.GetKeyValues(), token);
                    if (exists == null)
                    {
                        await set.AddAsync(e, token);
                        countInsert++;
                    }
                    else
                    {
                        db.Entry(exists).CurrentValues.SetValues(e);
                        countUpdated++;
                    }
                }
                await db.SaveChangesAsync(token);
            }, token);

            Console.WriteLine($"[UPSERT] {typeof(T).Name} {DateTime.Now:HH:mm:ss.fff} Added: {countInsert} Updated: {countUpdated}");
        }

        public async Task MarkCleanAsync(IEnumerable<T> entities, CancellationToken token)
        {
            await DbWriteExtensions.WriteAsync(async () =>
            {
                await using var db = await dbFactory.CreateDbContextAsync(token);
                var set = db.Set<T>();

                foreach (var e in entities)
                {
                    var item = await set.FindAsync(e.GetKeyValues(), token);
                    if (item != null)
                        item.IsDirty = false;
                }

                await db.SaveChangesAsync(token);
            }, token);
        }

        public async Task<T?> GetByIdAsync(params object?[]? keys)
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var set = db.Set<T>();
            return await set.FindAsync(keys);
        }

        public async Task DeleteAsync(T entity, CancellationToken token)
        {
            await DbWriteExtensions.WriteAsync(async () =>
            {
                await using var db = await dbFactory.CreateDbContextAsync(token);
                var set = db.Set<T>();

                set.Remove(entity);
                await db.SaveChangesAsync(token);
            }, token);
        }

        public IQueryable<T> Query()
        {
            // Query returned from newly created context; caller must enumerate immediately.
            var db = dbFactory.CreateDbContext();
            // Note: caller is responsible to dispose enumerator if necessary.

            return db.Set<T>().AsQueryable();
        }
    }
}
