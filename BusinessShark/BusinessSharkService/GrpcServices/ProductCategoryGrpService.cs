using BusinessSharkService.Handlers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class ProductCategoryGrpService(
        ILogger<ProductDefinitionGrpcService> logger, ProductCategoryHandler productCategoryHandler) : ProductCategoryService.ProductCategoryServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger = logger;

        public override async Task<ProductCategoryResponse> Sync(ProductCategoryRequest request, ServerCallContext context)
        {             
            var productCategories = await productCategoryHandler.LoadAsync(request.Timestamp.ToDateTime());
            var response = new ProductCategoryResponse();
            if (productCategories.Any())
            {
                response.ProductCategories.AddRange(productCategories.ConvertAll(pc => new ProductCategoryGrpc
                {
                    ProductCategoryId = pc.ProductCategoryId,
                    Name = pc.Name,
                    SortOrder = pc.SortOrder
                }));

                response.UpdatedAt = Timestamp.FromDateTime(productCategories.Max(p => p.UpdatedAt));
            }

            return response;
        }
    }
}
