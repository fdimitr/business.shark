using BusinessSharkClient.Logic.ViewModels;
using BusinessSharkService;

namespace BusinessSharkClient.Logic
{
    public class DivisionTransactionProvider
    {
        private DivisionTransactionsService.DivisionTransactionsServiceClient _transactionClient;
        public DivisionTransactionProvider(DivisionTransactionsService.DivisionTransactionsServiceClient transactionClient)
        {
            _transactionClient = transactionClient;
        }

        public async Task<DivisionAnalyticsViewModel> GetDivisionAnalytics(int divisionId)
        {
            var response = await _transactionClient.LoadAsync(
                new DivisionTransactionRequest { DivisionId = divisionId });
            
            var analytics = new DivisionAnalyticsViewModel(response.DivisionTransactions.ToList());
            return analytics;
        }

        public async Task<List<DivisionTransactionViewModel>> GetDivisionFinancialStatistics(int divisionId)
        {
            var response = await _transactionClient.LoadAsync(
                new DivisionTransactionRequest { DivisionId = divisionId });

            var result = new List<DivisionTransactionViewModel>();
            foreach (var grpcModel in response.DivisionTransactions.ToList())
            {
                var statistics = new DivisionTransactionViewModel
                {
                    DivisionTransactionsId = grpcModel.DivisionTransactionsId,
                    DivisionId = divisionId,
                    TransactionDate = grpcModel.TransactionDate.ToDateTime(),
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
