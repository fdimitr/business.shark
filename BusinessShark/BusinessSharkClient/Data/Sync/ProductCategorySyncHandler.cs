using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class ProductCategorySyncHandler(
        ILocalRepository<ProductCategoryEntity> repo,
        ProductCategoryService.ProductCategoryServiceClient remote,
        AppDbContext db,
        ILogger<ProductCategorySyncHandler> logger) : BaseSyncHandler(db), ISyncHandler<ProductCategoryEntity>
    {
        public override string EntityName => "ProductCategory";
        public SyncPriority Priority => SyncPriority.Critical;
        public Task<bool> PushAsync(CancellationToken token = default)
        {
            // ProductCategory is not pushed from client to server
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            ProductCategoryResponse? pull;
            try
            {
                pull = await remote.SyncAsync(new ProductCategoryRequest
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

            if (!pull.ProductCategories.Any()) return false;

            await using var tx = await dbContext.Database.BeginTransactionAsync(token);
            try
            {
                                // apply updated/inserted
                var upserts = pull.ProductCategories.Select(u => new ProductCategoryEntity
                {
                    Id = u.ProductCategoryId,
                    Name = u.Name,
                    IsDeleted = false,
                    IsDirty = false
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
