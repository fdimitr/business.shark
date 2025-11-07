using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Finance;
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
            result.DivisionTransactions.AddRange(transactions.ConvertAll(t => new DivisionTransactionsGrpc
            {

            }));
            return result;
        }
    }
}
