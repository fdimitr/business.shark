using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Logic.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Logic
{
    public class DivisionTransactionProvider(ILocalRepository<DivisionTransactionEntity> repo)
    {
        public async Task<List<DivisionTransactionEntity>> LoadAsync(int divisionId)
        {
            return await repo.Query().Where(x => x.DivisionId == divisionId).ToListAsync();
        }

        public async Task<DivisionAnalyticsViewModel> GetDivisionAnalytics(int divisionId)
        {
            var transactions = await repo.Query().Where(x => x.DivisionId == divisionId).ToListAsync();

            var analytics = new DivisionAnalyticsViewModel(transactions);
            return analytics;
        }

        public async Task<List<DivisionTransactionViewModel>> GetDivisionFinancialStatistics(int divisionId)
        {
            var transactions = await repo.Query().Where(x => x.DivisionId == divisionId).ToListAsync();

            var result = new List<DivisionTransactionViewModel>();
            foreach (var grpcModel in transactions)
            {
                var statistics = new DivisionTransactionViewModel
                {
                    DivisionTransactionsId = grpcModel.Id,
                    DivisionId = divisionId,
                    TransactionDate = grpcModel.TransactionDate,
                    SalesProductsAmount = grpcModel.SalesProductsAmount,
                    PurchasedProductsAmount = grpcModel.PurchasedProductsAmount,
                    TransportCostsAmount = grpcModel.TransportCostsAmount,
                    EmployeeSalariesAmount = grpcModel.EmployeeSalariesAmount,
                    MaintenanceCostsAmount = grpcModel.MaintenanceCostsAmount,
                    IncomeTaxAmount = grpcModel.IncomeTaxAmount,
                    RentalCostsAmount = grpcModel.RentalCostsAmount,
                    EmployeeTrainingAmount = grpcModel.EmployeeTrainingAmount,
                    CustomAmount = grpcModel.CustomAmount,
                    AdvertisingCostsAmount = grpcModel.AdvertisingCostsAmount,
                    ReplenishmentAmount = grpcModel.ReplenishmentAmount,
                    QuantityProduced = grpcModel.QuantityProduced,
                    QualityProduced = grpcModel.QualityProduced
                };

                result.Add(statistics);
            }
            return result;
        }
    }
}
