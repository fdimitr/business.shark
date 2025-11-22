using BusinessSharkService.Handlers.Finance;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class DivisionTransactionGrpcService(
        ILogger<DivisionTransactionGrpcService> logger,
        DivisionTransactionHandler divisionTransactionHandler) : DivisionTransactionsService.DivisionTransactionsServiceBase
    {
        private readonly ILogger<DivisionTransactionGrpcService> _logger = logger;

        public override async Task<DivisionTransactionsResponse> Sync(DivisionTransactionRequest request, ServerCallContext context)
        {
            var transactions = await divisionTransactionHandler.SyncAsync(request.CompanyId, request.Timestamp.ToDateTime());

            var response = new DivisionTransactionsResponse();

            if (transactions.Any())
            {
                response.DivisionTransactions.AddRange(transactions.OrderBy(t => t.TransactionDate).ToList().ConvertAll(
                    t => new DivisionTransactionsGrpc
                    {
                        DivisionTransactionsId = t.DivisionTransactionsId,
                        DivisionId = t.DivisionId,
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

                response.UpdatedAt = Timestamp.FromDateTime(transactions.Max(p => p.UpdatedAt));
            }

            return response;
        }
    }
}
