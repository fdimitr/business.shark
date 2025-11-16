using BusinessSharkService.Handlers.Finance;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class DivisionTransactionGrpcService : DivisionTransactionsService.DivisionTransactionsServiceBase
    {
        private readonly ILogger<DivisionTransactionGrpcService> _logger;
        private readonly DivisionTransactionHandler _divisionTransactionHandler;

        public DivisionTransactionGrpcService(ILogger<DivisionTransactionGrpcService> logger, DivisionTransactionHandler divisionTransactionHandler)
        {
            _logger = logger;
            _divisionTransactionHandler = divisionTransactionHandler;
        }

        public override async Task<DivisionTransactionsResponse> Load(DivisionTransactionRequest request, ServerCallContext context)
        {
            var transactions = await _divisionTransactionHandler.GetByDivisionAsync(request.DivisionId);
            var result = new DivisionTransactionsResponse();
            result.DivisionTransactions.AddRange(transactions?.OrderBy(t=>t.TransactionDate).ToList().ConvertAll(t => new DivisionTransactionsGrpc
            {
                DivisionTransactionsId = t.DivisionTransactionsId,
                AdvertisingCostsAmount = t.AdvertisingCostsAmount,
                CustomAmount = t.CustomAmount,
                EmployeeSalariesAmount = t.EmployeeSalariesAmount,
                EmployeeTrainingAmount = t.EmployeeTrainingAmount,
                IncomeTaxAmount = t.IncomeTaxAmount,
                MaintenanceCostsAmount = t.MaintenanceCostsAmount,
                PurchasedProductsAmount = t.PurchasedProductsAmount,
                QualityProduced = t.QualityProduced,
                QuantityProduced = t.QuantityProduced,
                RentalCostsAmount = t.RentalCostsAmount,
                ReplenishmentAmount = t.ReplenishmentAmount,
                SalesProductsAmount = t.SalesProductsAmount,
                TransactionDate = Timestamp.FromDateTime(t.TransactionDate),
                TransportCostsAmount = t.TransportCostsAmount
            }));
            return result;
        }
    }
}
