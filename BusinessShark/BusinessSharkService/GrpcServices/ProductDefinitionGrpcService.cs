using BusinessSharkService.Handlers;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class ProductDefinitionGrpcService : ProductDefinitionService.ProductDefinitionServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger;
        private readonly ProductDefinitionHandler _productDefinitionHandler;

        public ProductDefinitionGrpcService(ILogger<ProductDefinitionGrpcService> logger, ProductDefinitionHandler productDefinitionHandler)
        {
            _logger = logger;
            _productDefinitionHandler = productDefinitionHandler;
        }

        public async override Task<ProductDefinitionResponse> Sync(ProductDefinitionRequest request, ServerCallContext context)
        {
            var productDefinitions = await _productDefinitionHandler.PreloadProductDefinitionsAsync(request.Timestamp);

            var response = new ProductDefinitionResponse();
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
                Icon = ByteString.CopyFrom(GetImage(pd.IconPath)),

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

            return response;
        }

        private byte[] GetImage(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Array.Empty<byte>();

            try
            {
                // Resolve relative paths against the app base directory
                var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);

                // Normalize path
                fullPath = Path.GetFullPath(fullPath);

                if (!File.Exists(fullPath))
                {
                    _logger?.LogWarning("Image file not found: {Path}", fullPath);
                    return Array.Empty<byte>();
                }

                return File.ReadAllBytes(fullPath);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to load image at path '{Path}'", path);
                return Array.Empty<byte>();
            }
        }

    }
}
