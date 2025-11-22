using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class DivisionTransactionSyncHandler(
        ILocalRepository<DivisionTransactionEntity> repo,
        DataStateRepository repoDataState,
        DivisionTransactionsService.DivisionTransactionsServiceClient remote,
        ILogger<DivisionTransactionSyncHandler> logger) : BaseSyncHandler(repoDataState), ISyncHandler<DivisionTransactionEntity>
    {
        public SyncPriority Priority { get; } = SyncPriority.Normal;
        public override string EntityName => "DivisionTransactions";

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            DivisionTransactionsResponse? pull;
            try
            {
                pull = await remote.SyncAsync(new DivisionTransactionRequest
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

            if (!pull.DivisionTransactions.Any()) return false;

            try
            {
                // apply updated/inserted
                var upserts = pull.DivisionTransactions.Select(dt => new DivisionTransactionEntity
                {
                    Id = dt.DivisionTransactionsId,
                    DivisionId = dt.DivisionId,
                    TransactionDate = dt.TransactionDate.ToDateTime(),
                    SalesProductsAmount = dt.SalesProductsAmount,
                    PurchasedProductsAmount = dt.PurchasedProductsAmount,
                    TransportCostsAmount = dt.TransportCostsAmount,
                    EmployeeSalariesAmount = dt.EmployeeSalariesAmount,
                    MaintenanceCostsAmount = dt.MaintenanceCostsAmount,
                    IncomeTaxAmount = dt.IncomeTaxAmount,
                    RentalCostsAmount = dt.RentalCostsAmount,
                    EmployeeTrainingAmount = dt.EmployeeTrainingAmount,
                    CustomAmount = dt.CustomAmount,
                    AdvertisingCostsAmount = dt.AdvertisingCostsAmount,
                    ReplenishmentAmount = dt.ReplenishmentAmount,
                    QuantityProduced = dt.QuantityProduced,
                    QualityProduced = dt.QualityProduced
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
