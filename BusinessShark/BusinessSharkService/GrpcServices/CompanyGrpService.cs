using BusinessSharkService.Handlers;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class CompanyGrpService : CompanyService.CompanyServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger;
        private readonly CompanyHandler _companyHandler;

        public CompanyGrpService(ILogger<ProductDefinitionGrpcService> logger, CompanyHandler companyHandler)
        {
            _logger = logger;
            _companyHandler = companyHandler;
        }

        public override async Task<GetByPlayerResponse> GetByPlayer(GetByPlayerRequest request, ServerCallContext context)
        {
            var company = await _companyHandler.GetByPlayer(request.PlayerId);
            if (company == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Company not found"));

            var response = new GetByPlayerResponse
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
            };
            return response;
        }
    }
}
