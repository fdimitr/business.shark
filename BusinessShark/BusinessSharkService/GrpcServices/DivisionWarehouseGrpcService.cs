using System.Net.WebSockets;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Divisions;
using BusinessSharkService.Handlers.Finance;
using Google.Protobuf.Collections;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices
{
    public class DivisionWarehouseGrpcService : DivisionWarehouseService.DivisionWarehouseServiceBase
    {
        private readonly ILogger<DivisionWarehouseGrpcService> _logger;
        private readonly WarehouseHandler _warehouseHandler;
        private readonly WarehouseProductsHandler _warehouseProductsHandler;

        public DivisionWarehouseGrpcService(ILogger<DivisionWarehouseGrpcService> logger, WarehouseHandler warehouseHandler, WarehouseProductsHandler warehouseProductsHandler)
        {
            _logger = logger;
            _warehouseHandler = warehouseHandler;
            _warehouseProductsHandler = warehouseProductsHandler;
        }

        public override async Task<DivisionWarehouseResponse> Load(DivisionWarehouseRequest request, ServerCallContext context)
        {
            var result = new DivisionWarehouseResponse();
            var warehouse = await _warehouseHandler.GetByDivisionAsync(request.DivisionId, request.WarehouseType);
            if (warehouse == null) return result;

            result.DivisionWarehouseId = warehouse.WarehouseId;
            result.WarehouseCapacity = warehouse.VolumeCapacity;

            var products = await _warehouseProductsHandler.GetByWarehouseAsync(warehouse.WarehouseId);
            foreach (var p in products)
            {
                result.Products!.Add(new DivisionWarehouseProductsGrpc()
                {
                    WarehouseProductId = p.WarehouseProductId,
                    ProductDefinitionId = p.ProductDefinitionId,
                    Quantity = p.Quantity,
                    Quality = p.Quality,
                    CostPrice = p.CostPrice,
                    SalesPrice = p.SalesPrice,
                    SalesLimit = p.SalesLimit,
                    AvailableForSale = p.AvailableForSale
                });
            }


            return result;
        }

        public override async Task<DivisionWarehouseUpdateResponse> Update(DivisionWarehouseUpdateRequest request, ServerCallContext context)
        {
            var response = new DivisionWarehouseUpdateResponse();

            // Validate request
            if (request.DivisionWarehouseId <= 0 || request.Products.Count == 0)
            {
                response.Success = false;
                response.Message = "Invalid request.";
                return response;
            }

            // Update warehouse products
            foreach (var product in request.Products)
            {
                var updateResult = await _warehouseProductsHandler.UpdateProductAsync(
                    product.WarehouseProductId, product.SalesLimit, product.SalesPrice, product.AvailableForSale);
                if (!updateResult)
                {
                    response.Success = false;
                    response.Message = $"Failed to update product {product.WarehouseProductId}.";
                    return response;
                }
            }

            response.Success = true;
            response.Message = "Warehouse updated successfully.";
            return response;
        }
    }
}
