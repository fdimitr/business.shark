using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class CountrySyncHandler(
        ILocalRepository<CountryEntity> repo,
        CountryService.CountryServiceClient remote,
        AppDbContext db,
        ILogger<CountrySyncHandler> logger) : BaseSyncHandler(db), ISyncHandler<CountryEntity>
    {
        public override string EntityName => "Country";
        public SyncPriority Priority { get; } = SyncPriority.Critical;

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            CountrySyncResponse? pull;
            try
            {
                pull = await remote.SyncAsync(new CountrySyncRequest()
                {
                    Timestamp = lastSync != null
                        ? Timestamp.FromDateTime(lastSync.Value)
                        : Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime())
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Pull failed for {Entity}", EntityName);
                return false;
            }

            if (!pull.Countries.Any()) return false;

            await using var tx = await DbContext.Database.BeginTransactionAsync(token);
            try
            {
                // apply updated/inserted
                var upserts = pull.Countries.Select(c => new CountryEntity
                {
                    Id = c.CountryId,
                    Code = c.Code,
                    Name = c.Name,
                    IsDeleted = false,
                    IsDirty = false,
                });

                await repo.UpsertRangeAsync(upserts, token);

                await SetLastSyncAsync(pull.UpdatedAt.ToDateTime());
                await tx.CommitAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(token);
                logger.LogError(ex, "Pull failed for {Entity}", EntityName);
                return false;
            }
        }
    }
}
