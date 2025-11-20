using BusinessSharkService.Handlers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class ProductDefinitionGrpcService(
        ILogger<ProductDefinitionGrpcService> logger, ProductDefinitionHandler productDefinitionHandler) : ProductDefinitionService.ProductDefinitionServiceBase
    {
        public override async Task<ProductDefinitionResponse> Sync(ProductDefinitionRequest request, ServerCallContext context)
        {
            var productDefinitions = await productDefinitionHandler.PreloadProductDefinitionsAsync(request.Timestamp.ToDateTime());

            var response = new ProductDefinitionResponse();
            if (productDefinitions.Any())
            {
                response.ProductDefinitions.AddRange(productDefinitions.ConvertAll(pd => new ProductDefinitionGrpc
                {
                    ProductDefinitionId = pd.ProductDefinitionId,
                    ProductCategoryId = pd.ProductCategoryId,
                    Name = pd.Name,
                    Volume = pd.Volume,
                    BaseProductionCount = pd.BaseProductionCount,
                    BaseProductionPrice = (double)pd.BaseProductionPrice,
                    TechImpactQuality = pd.TechImpactQuality,
                    ToolImpactQuality = pd.ToolImpactQuality,
                    WorkerImpactQuality = pd.WorkerImpactQuality,

                    TechImpactQuantity = pd.TechImpactQuantity,
                    ToolImpactQuantity = pd.ToolImpactQuantity,
                    WorkerImpactQuantity = pd.WorkerImpactQuantity,

                    DeliveryPrice = (double)pd.DeliveryPrice,
                    Image = ByteString.CopyFrom(GetImage(pd.ImagePath)),

                    ComponentUnits =
                    {
                        pd.ComponentUnits.ConvertAll(cu => new ComponentUnitGrpc
                        {
                            ProductionQuantity = cu.ProductionQuantity,
                            ComponentDefinitionId = cu.ComponentDefinitionId,
                            QualityImpact = cu.QualityImpact,
                        })
                    },
                }));

                response.UpdatedAt = Timestamp.FromDateTime(productDefinitions.Max(p => p.UpdatedAt));
            }

            return response;
        }

        private byte[] GetImage(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return [];

            try
            {
                // Resolve relative paths against the app base directory
                var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);

                // Normalize path
                fullPath = Path.GetFullPath(fullPath);

                if (!File.Exists(fullPath))
                {
                    logger.LogWarning("Image file not found: {Path}", fullPath);
                    return [];
                }

                return File.ReadAllBytes(fullPath);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to load image at path '{Path}'", path);
                return [];
            }
        }

    }
}
