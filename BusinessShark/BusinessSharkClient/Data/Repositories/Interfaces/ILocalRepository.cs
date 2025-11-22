using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Repositories.Interfaces
{
    public interface ILocalRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetDirtyAsync(int batchSize = 100);
        Task UpsertAsync(T entity, CancellationToken token);
        Task UpsertRangeAsync(IEnumerable<T> entities, CancellationToken token);
        Task MarkCleanAsync(IEnumerable<T> entities, CancellationToken token);
        Task<T?> GetByIdAsync(params object[] keys);
        Task DeleteAsync(T entity, CancellationToken token);
        IQueryable<T> Query();
    }
}
