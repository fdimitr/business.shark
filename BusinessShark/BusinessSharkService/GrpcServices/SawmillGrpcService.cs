using BusinessSharkService.Handlers.Divisions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices
{
    public class SawmillGrpcService : SawmillService.SawmillServiceBase
    {
        private readonly ILogger<ProductDefinitionGrpcService> _logger;
        private readonly SawmillHandler _sawmillHandler;

        public SawmillGrpcService(ILogger<ProductDefinitionGrpcService> logger,  SawmillHandler sawmillHandler)
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
                CountryCode = s.City != null ? s.City.Country.Code : string.Empty,
                ProductDefinitionId = s.ProductDefinitionId,
                VolumeCapacity = s.DivisionSize?.Size ?? 0
            }));
            return response;

        }

        public override async Task<SawmillResponse> LoadDetail(SawmillRequest request, ServerCallContext context)
        {
            var sawmill = await _sawmillHandler.LoadAsync(request.DivisionId);
            if (sawmill == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Sawmill with DivisionId {request.DivisionId} not found."));
            }
            var response = new SawmillResponse
            {
                DivisionId = sawmill.DivisionId,
                Name = sawmill.Name,
                City = sawmill.City != null ? sawmill.City.Name : string.Empty,
                CountryCode = sawmill.City != null ? sawmill.City.Country.Code : string.Empty,
                ProductDefinitionId = sawmill.ProductDefinitionId,
                VolumeCapacity = sawmill.DivisionSize?.Size ?? 0,
                ResourceDepositQuality = sawmill.ResourceDepositQuality,
                RawMaterialReserves = sawmill.RawMaterialReserves,
                TechLevel = sawmill.TechLevel,
                PlantingCosts = sawmill.PlantingCosts,
                QualityBonus = sawmill.QualityBonus,
                QuantityBonus = sawmill.QuantityBonus,
                OutputWarehouse = new SawmillWarhouseGrpc
                {
                    WarehouseId = sawmill.OutputWarehouse.WarehouseId,
                    VolumeCapacity = sawmill.OutputWarehouse.VolumeCapacity,
                    Products = { sawmill.OutputWarehouse.Products != null ? sawmill.OutputWarehouse.Products.ConvertAll(p => new SawmillWarehouseProductsGrpc
                    {
                        WarehouseProductId = p.WarehouseProductId,
                        ProductDefinitionId = p.ProductDefinitionId,
                        Quantity = p.Quantity,
                        Quality = p.Quality,
                        UnitPrice = p.UnitPrice
                    }) : new List<SawmillWarehouseProductsGrpc>()
                    }
                },
                Tools = sawmill.Tools != null ? new SawmillToolsGrpc
                {
                    ToolsId = sawmill.Tools.ToolsId,
                    TotalQuantity = sawmill.Tools.TotalQuantity,
                    TechLevel = sawmill.Tools.TechLevel,
                    Deprecation = sawmill.Tools.Deprecation,
                    MaintenanceCostsAmount = sawmill.Tools.MaintenanceCostsAmount
                } : null,
                Employees = sawmill.Employees != null ? new SawmillEmployeesGrpc
                {
                    EmployeesId = sawmill.Employees.EmployeesId,
                    TotalQuantity = sawmill.Employees.TotalQuantity,
                    SalaryPerEmployee = sawmill.Employees.SalaryPerEmployee,
                    SkillLevel = sawmill.Employees.SkillLevel
                } : null,
                DivisionTransactions =  { sawmill.DivisionTransactions != null ? sawmill.DivisionTransactions.ConvertAll(t => new DivisionTransactionsGrpc
                {
                    DivisionTransactionsId = t.DivisionTransactionsId,
                    TransactionDate = Timestamp.FromDateTime(t.TransactionDate),
                    SalesProductsAmount = t.SalesProductsAmount,
                    PurchasedProductsAmount = t.PurchasedProductsAmount,
                    TransportCostsAmount = t.TransportCostsAmount,
                    EmployeeSalariesAmount = t.EmployeeSalariesAmount,
                    MaintenanceCostsAmount = t.MaintenanceCostsAmount,
                    IncomeTaxAmount = t.IncomeTaxAmount,
                    RentalCostsAmount = t.RentalCostsAmount,
                    EmployeeTrainingAmount = t.EmployeeTrainingAmount,
                    CustomAmount = t.CustomAmount,
                    AdvertisingCostsAmount = t.AdvertisingCostsAmount,
                    QualityProduced = t.QualityProduced,
                    QuantityProduced = t.QuantityProduced,
                }) : new List<DivisionTransactionsGrpc>()
                }
            };

            return response;
        }
    }

}
