using BusinessSharkService.DataAccess;
using BusinessSharkService.Handlers.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class SummaryHandler
    {
        private readonly DataContext _dbContext;
        public SummaryHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CurrentSummary> LoadAsync(int playerId)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(p => p.PlayerId == playerId);
            if (company == null)
            {
                throw new Exception("Player not found");
            }

            var lastTransaction = await _dbContext.FinancialTransactions
                .Where(t => t.CompanyId == company.CompanyId)
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefaultAsync();

            return new CurrentSummary
            {
                Balance = company.Balance,
                Income = lastTransaction?.SalesProductsAmount ?? 0,
                Expenses = lastTransaction != null ? lastTransaction.PurchasedProductsAmount +
                           lastTransaction.TransportCostsAmount +
                           lastTransaction.EmployeeSalariesAmount +
                           lastTransaction.MaintenanceCostsAmount +
                           lastTransaction.IncomeTaxAmount +
                           lastTransaction.RentalCostsAmount +
                           lastTransaction.EmployeeTrainingAmount +
                           lastTransaction.CustomAmount +
                           lastTransaction.AdvertisingCostsAmount +
                           lastTransaction.RentalCostsAmount : 0
            };
        }
    }
}
