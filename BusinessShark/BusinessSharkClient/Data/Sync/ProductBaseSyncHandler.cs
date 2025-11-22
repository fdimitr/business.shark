using BusinessSharkClient.Data.Entities;

namespace BusinessSharkClient.Data.Sync
{
    public class BaseSyncHandler(AppDbContext db)
    {
        protected readonly AppDbContext DbContext = db;

        public virtual string EntityName => string.Empty;

        // Helpers for lastSync (используйте таблицу AppState)
        protected async Task<DateTime?> GetLastSyncAsync()
        {
            var appState = await DbContext.DataStates.FindAsync($"{EntityName}_lastsync");
            if (appState == null) return null;
            return DateTime.SpecifyKind(appState.Value, DateTimeKind.Utc);
        }

        protected async Task SetLastSyncAsync(DateTime dt)
        {
            var key = $"{EntityName}_lastsync";
            var appState = await DbContext.DataStates.FindAsync(key);
            if (appState == null)
            {
                DbContext.DataStates.Add(new DataState { Key = key, Value = dt });
            }
            else
            {
                appState.Value = dt;
            }
            await DbContext.SaveChangesAsync();
        }
    }
}
