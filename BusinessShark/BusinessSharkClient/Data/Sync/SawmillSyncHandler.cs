using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class SawmillSyncHandler(ILocalRepository<SawmillEntity> repo,
        SawmillService.SawmillServiceClient remote,
        AppDbContext db,
        ILogger<SawmillSyncHandler> logger) : BaseSyncHandler(db), ISyncHandler<SawmillEntity>
    {
        public override string EntityName => "Sawmills";
        public SyncPriority Priority => SyncPriority.High;

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            // Sawmills is not pushed from client to server
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            var pull = await remote.SyncAsync(new SawmillSyncRequest
            {
                CompanyId = companyId,
                Timestamp = lastSync != null
                    ? Timestamp.FromDateTime(lastSync.Value)
                    : Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime())
            });

            if (!pull.Sawmills.Any()) return false;

            await using var tx = await dbContext.Database.BeginTransactionAsync(token);
            try
            {
                var upserts = pull.Sawmills.Select(s => new SawmillEntity
                {
                    Id = s.DivisionId,
                    Name = s.Name,
                    CountryCode = s.CountryCode,
                    City = s.City,
                    CompanyId = companyId,
                    ProductDefinitionId = s.ProductDefinitionId,
                    VolumeCapacity = s.VolumeCapacity,
                    Description = s.Description,
                    ResourceDepositQuality = s.ResourceDepositQuality,
                    RawMaterialReserves = s.RawMaterialReserves,
                    TechLevel = s.TechLevel,
                    PlantingCosts = s.PlantingCosts,
                    RentalCost = s.RentalCost,
                    QuantityBonus = s.QuantityBonus,
                    QualityBonus = s.QualityBonus
                });

                await repo.UpsertRangeAsync(upserts);
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
