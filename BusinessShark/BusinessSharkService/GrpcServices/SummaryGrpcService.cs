using BusinessSharkService.Handlers;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class SummaryGrpcService : SummaryService.SummaryServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger;
        private readonly SummaryHandler _summaryHandler;

        public SummaryGrpcService(ILogger<ProductDefinitionGrpcService> logger, SummaryHandler summaryHandler)
        {
            _logger = logger;
            _summaryHandler = summaryHandler;
        }

        public override async Task<SummaryResponse> Load(SummaryRequest request, ServerCallContext context)
        {  
            var summary = await _summaryHandler.LoadAsync(request.PlayerId);
            return new SummaryResponse
            {
                Balance = summary.Balance,
                Income = summary.Income,
                Expenses = summary.Expenses
            };
        }
    }
}
