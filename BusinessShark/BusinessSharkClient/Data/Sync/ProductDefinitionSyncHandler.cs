using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class ProductDefinitionSyncHandler(
        ILocalRepository<ClientProductDefinition> repo,
        ProductDefinitionService.ProductDefinitionServiceClient remote,
        AppDbContext db,
        ILogger<ProductDefinitionSyncHandler> logger) : ISyncHandler<ClientProductDefinition>
    {
        private readonly ILocalRepository<ClientProductDefinition> _repo = repo;
        private ProductDefinitionService.ProductDefinitionServiceClient _remote = remote;
        private readonly AppDbContext _db = db;
        private readonly ILogger<ProductDefinitionSyncHandler> _logger = logger;
        private const int BatchSize = 50;

        public string EntityName => "ProductDefinition";
        public Task<bool> PushAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PullAsync(CancellationToken token = default)
        {
            // Получаем lastSync из AppState
            var lastSync = await GetLastSyncAsync();

            var pull = await _remote.SyncAsync(new ProductDefinitionRequest { Timestamp = (uint)(lastSync ?? 0) });

            if (!pull.ProductDefinitions.Any()) return false;

            // Обновляем локально в транзакции
            await using var tx = await _db.Database.BeginTransactionAsync(token);
            try
            {
                // apply updated/inserted
                var upserts = pull.ProductDefinitions.Select(u => new ClientProductDefinition
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
                    UpdatedAt = DateTime.UtcNow,
                    TimeStamp = pull.Timestamp,
                    Image = u.Image.ToByteArray()
                });

                await _repo.UpsertRangeAsync(upserts);

                await SetLastSyncAsync(pull.Timestamp);
                await tx.CommitAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(token);
                _logger.LogError(ex, "Pull failed for {Entity}", EntityName);
                return false;
            }
        }

        // Helpers for lastSync (используйте таблицу AppState)
        private async Task<uint?> GetLastSyncAsync()
        {
            var appState = await _db.DataStates.FindAsync($"{EntityName}_lastsync");
            if (appState == null) return null;
            return appState.Value;
        }

        private async Task SetLastSyncAsync(uint ts)
        {
            var key = $"{EntityName}_lastsync";
            var appState = await _db.DataStates.FindAsync(key);
            if (appState == null)
            {
                _db.DataStates.Add(new DataState { Key = key, Value = ts });
            }
            else
            {
                appState.Value = ts;
            }
            await _db.SaveChangesAsync();
        }
    }
}
