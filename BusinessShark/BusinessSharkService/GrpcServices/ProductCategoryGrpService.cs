using BusinessSharkService.Handlers;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace BusinessSharkService.GrpcServices
{
    [Authorize]
    public class ProductCategoryGrpService : ProductCategoryService.ProductCategoryServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger;
        private readonly ProductCategoryHandler _productCategoryHandler;

        public ProductCategoryGrpService(ILogger<ProductDefinitionGrpcService> logger, ProductCategoryHandler productCategoryHandler)
        {
            _logger = logger;
            _productCategoryHandler = productCategoryHandler;
        }

        public async override Task<ProductCategoryResponse> Load(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
        {             
            var productCategories = await _productCategoryHandler.LoadAsync();
            var response = new ProductCategoryResponse();
            response.ProductCategories.AddRange(productCategories.ConvertAll(pc => new ProductCategoryGrpc
            {
                ProductCategoryId = pc.ProductCategoryId,
                Name = pc.Name,
                SortOrder = pc.SortOrder
            }));
            return response;
        }
    }
}
