using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class ProductDefinitionSyncHandler(
        ILocalRepository<ProductDefinitionEntity> repo,
        ProductDefinitionService.ProductDefinitionServiceClient remote,
        AppDbContext db,
        ILogger<ProductDefinitionSyncHandler> logger) : BaseSyncHandler(db), ISyncHandler<ProductDefinitionEntity>
    {
        public override string EntityName => "ProductDefinition";
        public SyncPriority Priority => SyncPriority.Critical;

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            // ProductDefinition is not pushed from client to server
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            var pull = await remote.SyncAsync(new ProductDefinitionRequest
            {
                Timestamp = lastSync != null
                    ? Timestamp.FromDateTime(lastSync.Value)
                    : Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime())
            });

            if (!pull.ProductDefinitions.Any()) return false;

            // Обновляем локально в транзакции
            await using var tx = await dbContext.Database.BeginTransactionAsync(token);
            try
            {
                // apply updated/inserted
                var upserts = pull.ProductDefinitions.Select(u => new ProductDefinitionEntity
                {
                    Id = u.ProductDefinitionId,
                    Name = u.Name,
                    IsDeleted = false,
                    IsDirty = false,
                    ProductCategoryId = u.ProductCategoryId,
                    Volume = u.Volume,
                    BaseProductionCount = u.BaseProductionCount,
                    BaseProductionPrice = u.BaseProductionPrice,
                    TechImpactQuality = u.TechImpactQuality,
                    ToolImpactQuality = u.ToolImpactQuality,
                    WorkerImpactQuality = u.WorkerImpactQuality,
                    TechImpactQuantity = u.TechImpactQuantity,
                    ToolImpactQuantity = u.ToolImpactQuantity,
                    WorkerImpactQuantity = u.WorkerImpactQuantity,
                    DeliveryPrice = u.DeliveryPrice,
                    Image = u.Image.ToByteArray(),
                    ComponentUnits = u.ComponentUnits.Select(cuGrpc => new ComponentUnitEntity
                    {
                        ProductDefinitionId = u.ProductDefinitionId,
                        Id = cuGrpc.ComponentDefinitionId,
                        ProductionQuantity = cuGrpc.ProductionQuantity,
                        QualityImpact = cuGrpc.QualityImpact
                    }).ToList()
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
