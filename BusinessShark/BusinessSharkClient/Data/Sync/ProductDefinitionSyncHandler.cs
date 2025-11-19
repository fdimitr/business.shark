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
        ILogger<ProductDefinitionSyncHandler> logger) : ISyncHandler<ProductDefinitionEntity>
    {
        public string EntityName => "ProductDefinition";

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PullAsync(CancellationToken token = default)
        {
            // Получаем lastSync из AppState
            var lastSync = await GetLastSyncAsync();

            var pull = await remote.SyncAsync(new ProductDefinitionRequest
            {
                Timestamp = lastSync != null
                    ? Timestamp.FromDateTime(lastSync.Value)
                    : Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime())
            });

            if (!pull.ProductDefinitions.Any()) return false;

            // Обновляем локально в транзакции
            await using var tx = await db.Database.BeginTransactionAsync(token);
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
                    UpdatedAt = pull.UpdatedAt.ToDateTime(),
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

        // Helpers for lastSync (используйте таблицу AppState)
        private async Task<DateTime?> GetLastSyncAsync()
        {
            var appState = await db.DataStates.FindAsync($"{EntityName}_lastsync");
            if (appState == null) return null;
            return DateTime.SpecifyKind(appState.Value, DateTimeKind.Utc);
        }

        private async Task SetLastSyncAsync(DateTime dt)
        {
            var key = $"{EntityName}_lastsync";
            var appState = await db.DataStates.FindAsync(key);
            if (appState == null)
            {
                db.DataStates.Add(new DataState { Key = key, Value = dt });
            }
            else
            {
                appState.Value = dt;
            }
            await db.SaveChangesAsync();
        }
    }
}
