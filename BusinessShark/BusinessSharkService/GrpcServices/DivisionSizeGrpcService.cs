using BusinessSharkService.Handlers;

namespace BusinessSharkService.GrpcServices
{
    public class DivisionSizeGrpcService : DivisionSizeService.DivisionSizeServiceBase
    {
        private readonly ILogger<DivisionSizeGrpcService> _logger;
        private readonly DivisionSizeHandler _divisionSizeHandler;

        public DivisionSizeGrpcService(ILogger<DivisionSizeGrpcService> logger, DivisionSizeHandler divisionSizeHandler)
        {
            _logger = logger;
            _divisionSizeHandler = divisionSizeHandler;
        }

        public override async Task<DivisionSizeResponse> Load(DivisionSizeRequest request, Grpc.Core.ServerCallContext context)
        {
            var divisionSizes = await _divisionSizeHandler.GetDivisionSizesAsync(request.DivisionTypeId);
            var result = new DivisionSizeResponse();
            result.Sizes.AddRange(divisionSizes.ConvertAll(ds => new DivisionSizeGrpc
            {
                DivisionSizeId = ds.DivisionSizeId,
                Size = ds.Size,
                ConstructionCost = ds.ConstructionCost,
                MaxEmployees = ds.MaxEmployeesQuantity,
                MaxTools = ds.MaxToolsQuantity,
                WarehouseVolume = ds.WarehouseVolume
            }));
            return result;
        }
    }
}
