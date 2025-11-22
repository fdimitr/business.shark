using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class CitySyncHandler(
        ILocalRepository<CityEntity> repo,
        DataStateRepository repoDataState,
        CityService.CityServiceClient remote,
        ILogger<CitySyncHandler> logger) : BaseSyncHandler(repoDataState), ISyncHandler<CityEntity>
    {
        public SyncPriority Priority { get; } = SyncPriority.Critical;
        public override string EntityName => "City";

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            CitySyncResponse? pull;
            try
            {
                pull = await remote.SyncAsync(new CitySyncRequest
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

            if (!pull.Cities.Any()) return false;

            try
            {
                // apply updated/inserted
                var upserts = pull.Cities.Select(c => new CityEntity
                {
                    Id = c.CityId,
                    Name = c.Name,
                    IsDeleted = false,
                    IsDirty = false,
                    CountryId = c.CountryId,
                    Population = c.Population,
                    AverageSalary = c.AverageSalary,
                    BaseLandPrice = c.BaseLandPrice,
                    LandTax = c.LandTax,
                    WealthLevel = c.WealthLevel,
                    Happiness = c.Happiness
                });

                await repo.UpsertRangeAsync(upserts, token);

                await SetLastSyncAsync(pull.UpdatedAt.ToDateTime());
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Pull failed for {Entity}", EntityName);
                return false;
            }
        }
    }
}
