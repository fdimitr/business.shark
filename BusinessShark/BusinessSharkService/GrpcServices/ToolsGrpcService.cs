using BusinessSharkService.Handlers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class ToolsGrpcService(
        ILogger<ToolsGrpcService> logger, ToolsHandler toolsHandler) : ToolsService.ToolsServiceBase
    {
        public override async Task<ToolsSyncResponse> Sync(ToolsSyncRequest request, ServerCallContext context)
        {
            var tools = await toolsHandler.LoadAsync(request.Timestamp.ToDateTime());
            var response = new ToolsSyncResponse();
            if (tools.Any())
            {
                response.Tools.AddRange(tools.ConvertAll(t=> new ToolsGrpc
                {
                    ToolsId = t.ToolsId,
                    DivisionId = t.DivisionId,
                    Quantity = t.TotalQuantity,
                    MaxQuantity = t.MaxQuantity,
                    TechLevel = t.TechLevel,
                    Wear = t.WearCoefficient,
                    Efficiency = t.Efficiency,
                    MaintenanceCosts = t.MaintenanceCostsAmount,
                    WarrantyDays = t.WarrantyDays
                }));

                response.UpdatedAt = Timestamp.FromDateTime(tools.Max(p => p.UpdatedAt));
            }

            return response;
        }
    }
}
