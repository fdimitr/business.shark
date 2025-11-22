using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkService;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

namespace BusinessSharkClient.Data.Sync
{
    public class EmployeesSyncHandler(
        ILocalRepository<EmployeesEntity> repo,
        DataStateRepository repoDataState,
        EmployeesService.EmployeesServiceClient remote,
        ILogger<EmployeesSyncHandler> logger) : BaseSyncHandler(repoDataState), ISyncHandler<EmployeesEntity>
    {
        public SyncPriority Priority { get; } = SyncPriority.High;
        public override string EntityName => "Employees";

        public Task<bool> PushAsync(CancellationToken token = default)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> PullAsync(int companyId, CancellationToken token = default)
        {
            // Get lastSync from DataState
            var lastSync = await GetLastSyncAsync();

            EmployeesSyncResponse? pull;
            try
            {
                pull = await remote.SyncAsync(new EmployeesSyncRequest
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

            if (!pull.Employees.Any()) return false;
            try
            {
                // apply updated/inserted
                var upserts = pull.Employees.Select(e => new EmployeesEntity
                {
                    Id = e.EmployeesId,
                    IsDeleted = false,
                    IsDirty = false,
                    DivisionId = e.DivisionId,
                    Salary = e.Salary,
                    SkillLevel = e.SkillLevel,
                    TotalQuantity = e.TotalQuantity
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
