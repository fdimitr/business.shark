using BusinessSharkClient.Data.Entities.Interfaces;
using BusinessSharkClient.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Data.Repositories
{
    public class EfLocalRepository<T>(AppDbContext db) : ILocalRepository<T> where T : class, IEntity
    {
        private readonly DbSet<T> _set = db.Set<T>();

        public async Task<List<T>> GetAllAsync()
            => await _set.AsNoTracking().OrderBy(x => x.Id).ToListAsync();

        public async Task<List<T>> GetDirtyAsync(int batchSize = 100)
            => await _set.Where(x => x.IsDirty)
                         .OrderBy(x => x.Id)
                         .Take(batchSize)
                         .ToListAsync();

        public async Task UpsertAsync(T entity, CancellationToken token)
        {
            await DbWriteExtensions.WriteAsync(async () =>
            {
                var exists = await _set.FindAsync(entity.Id, token);
                if (exists == null)
                    await _set.AddAsync(entity, token);
                else
                    db.Entry(exists).CurrentValues.SetValues(entity);

                await db.SaveChangesAsync(token);
            }, token);
        }

        public async Task UpsertRangeAsync(IEnumerable<T> entities, CancellationToken token)
        {
            await DbWriteExtensions.WriteAsync(async () =>
            {
                foreach (var e in entities)
                {
                    var exists = await _set.FindAsync(e.Id, token);
                    if (exists == null)
                        await _set.AddAsync(e, token);
                    else
                        db.Entry(exists).CurrentValues.SetValues(e);
                }

                await db.SaveChangesAsync(token);
            }, token);
        }

        public async Task MarkCleanAsync(IEnumerable<T> entities, CancellationToken token)
        {
            await DbWriteExtensions.WriteAsync(async () =>
            {
                foreach (var e in entities)
                {
                    var item = await _set.FindAsync(e.Id, token);
                    if (item != null)
                        item.IsDirty = false;
                }

                await db.SaveChangesAsync(token);
            }, token);
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _set.FindAsync(id);

        public async Task DeleteAsync(T entity, CancellationToken token)
        {
            await DbWriteExtensions.WriteAsync(async () =>
            {
                _set.Remove(entity);
                await db.SaveChangesAsync(token);
            }, token);
        }

        public IQueryable<T> Query() => _set.AsQueryable();
    }
}
