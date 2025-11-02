using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Divisions;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices
{
    public class SawmillGrpcService : SawmillService.SawmillServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger;
        private readonly SawmillHandler _sawmillHandler;

        public SawmillGrpcService(ILogger<ProductDefinitionGrpcService> logger, SawmillHandler sawmillHandler)
        {
            _logger = logger;
            _sawmillHandler = sawmillHandler;
        }

        public override async Task<SawmillListResponse> LoadList(SawmillListRequest request, ServerCallContext context)
        {
            var sawmills = await _sawmillHandler.LoadListAsync(request.CompanyId);
            var response = new SawmillListResponse();
            response.Sawmills.AddRange(sawmills.ConvertAll(s => new SawmillListGrpc
            {
                DivisionId = s.DivisionId,
                Name = s.Name,
                City = s.City != null ? s.City.Name : string.Empty,
                CountryCode = s.City != null && s.City.Country != null ? s.City.Country.Code : string.Empty,
                ProductDefinitionId = s.ProductDefinitionId,
                VolumeCapacity = s.VolumeCapacity
            }));
            return response;

        }
    }

}
