using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Sync.Interfaces
{
    public interface ISyncHandler
    {
        string EntityName { get; }
        SyncPriority Priority { get; }
        Task<bool> PushAsync(CancellationToken token = default);
        Task<bool> PullAsync(CancellationToken token = default);
    }

    public interface ISyncHandler<T> : ISyncHandler where T : class, IEntity
    {
    }
}
