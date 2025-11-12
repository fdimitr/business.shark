using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessSharkClient.Logic.Models;
using BusinessSharkService;
using BusinessSharkShared;
using CommunityToolkit.Maui.Core.Extensions;

namespace BusinessSharkClient.Logic
{
    public class DivisionWarehouseProvider
    {
        private readonly DivisionWarehouseService.DivisionWarehouseServiceClient _warehouseServiceClient;

        public DivisionWarehouseProvider(DivisionWarehouseService.DivisionWarehouseServiceClient warehouseServiceClient)
        {
            _warehouseServiceClient = warehouseServiceClient;
        }

        public async Task< DivisionWarehouseModel> GetDivisionWarehouseAsync(int divisionId)
        {
            var response = await _warehouseServiceClient.LoadAsync(
                new DivisionWarehouseRequest { DivisionId = divisionId, WarehouseType = (int)WarehouseType.Output });

            var result = new DivisionWarehouseModel
            {
                DivisionId = divisionId,
                DivisionWarehouseId = response.DivisionWarehouseId,
                WarehouseCapacity = response.WarehouseCapacity,
                Products = response.Products.Select(p => new WarehouseProductModel
                {
                    WarehouseProductId = p.WarehouseProductId,
                    ProductDefinitionId = p.ProductDefinitionId,
                    Quantity = p.Quantity,
                    Quality = p.Quality,
                    CostPrice = p.CostPrice,
                    SalesPrice = p.SalesPrice,
                    SalesLimit = p.SalesLimit,
                    AvailableForSale = p.AvailableForSale
                }).ToObservableCollection()
            };

            return result;
        }

        public async Task<DivisionWarehouseUpdateResponse> UpdatedDivisionWarehouseAsync(DivisionWarehouseModel divisionWarehouseModel)
        {
            var result = new DivisionWarehouseUpdateRequest
            {
                DivisionWarehouseId = divisionWarehouseModel.DivisionWarehouseId,
            };
            foreach (var product in divisionWarehouseModel.Products)
            {
                if (product.isChanged)
                {
                    result.Products.Add(new DivisionWarehouseUpdateProductsGrpc
                    {
                        WarehouseProductId = product.WarehouseProductId,
                        SalesPrice = product.SalesPrice,
                        SalesLimit = product.SalesLimit,
                        AvailableForSale = product.AvailableForSale
                    });
                }
            }

            if (result.Products.Any())
            {
                return await _warehouseServiceClient.UpdateAsync(result);
            }

            return new DivisionWarehouseUpdateResponse();
        }
    }
}
