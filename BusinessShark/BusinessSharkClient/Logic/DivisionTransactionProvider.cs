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

        public async Task<DivisionAnalyticsViewModel> GetDivisionTransactions(int divisionId)
        {
            var response = await _transactionClient.LoadAsync(
                new DivisionTransactionRequest { DivisionId = divisionId });
            
            var analytics = new DivisionAnalyticsViewModel(response.DivisionTransactions.ToList());
            return analytics;
        }
    }
}
