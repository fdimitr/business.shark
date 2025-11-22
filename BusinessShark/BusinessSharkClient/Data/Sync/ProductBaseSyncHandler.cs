using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories;

namespace BusinessSharkClient.Data.Sync
{
    public class BaseSyncHandler(DataStateRepository repoDataState)
    {
        public virtual string EntityName => string.Empty;

        // Helpers for lastSync (используйте таблицу DataState)
        protected async Task<DateTime?> GetLastSyncAsync()
        {
            var key = $"{EntityName}_lastsync";
            var appState = await repoDataState.GetByIdAsync(key);
            if (appState == null) return null;
            return DateTime.SpecifyKind(appState.Value, DateTimeKind.Utc);
        }

        protected async Task SetLastSyncAsync(DateTime dt)
        {
            var key = $"{EntityName}_lastsync";
            await repoDataState.UpsertAsync(new DataState { Key = key, Value = dt });
        }
    }
}
