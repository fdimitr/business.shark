using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class ToolsSyncHandler(
        ILocalRepository<ToolsEntity> repo,
        ToolsService.ToolsServiceClient remote,
        AppDbContext db,
        ILogger<ToolsSyncHandler> logger) : BaseSyncHandler(db), ISyncHandler<ToolsEntity>
    {
        public SyncPriority Priority { get; } = SyncPriority.High;
        public override string EntityName => "Tools";

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            ToolsSyncResponse? pull;
            try
            {
                pull = await remote.SyncAsync(new ToolsSyncRequest
                {
                    CompanyId = companyId,
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

            if (!pull.Tools.Any()) return false;

            await using var tx = await DbContext.Database.BeginTransactionAsync(token);
            try
            {
                // apply updated/inserted
                var upserts = pull.Tools.Select(t => new ToolsEntity
                {
                    Id = t.ToolsId,
                    IsDeleted = false,
                    IsDirty = false,
                    Quantity = t.Quantity,
                    DivisionId = t.DivisionId,
                    Efficiency = t.Efficiency,
                    MaintenanceCosts = t.MaintenanceCosts,
                    MaxQuantity = t.MaxQuantity,
                    TechLevel = t.TechLevel,
                    Wear = t.Wear,
                    WarrantyDays = t.WarrantyDays
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
