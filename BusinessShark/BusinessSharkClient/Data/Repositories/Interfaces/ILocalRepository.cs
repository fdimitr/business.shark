using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Repositories.Interfaces
{
    public interface ILocalRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetDirtyAsync(int batchSize = 100);
        Task UpsertAsync(T entity);
        Task UpsertRangeAsync(IEnumerable<T> entities);
        Task MarkCleanAsync(IEnumerable<T> entities);
        Task<T?> GetByIdAsync(int id);
        Task DeleteAsync(T entity);
        IQueryable<T> Query();
    }
}
