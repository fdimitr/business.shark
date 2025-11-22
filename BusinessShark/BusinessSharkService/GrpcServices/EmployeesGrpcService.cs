using BusinessSharkService.Handlers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices
{
    public class EmployeesGrpcService(
        ILogger<EmployeesGrpcService> logger, EmployeesHandler employeesHandler) : EmployeesService.EmployeesServiceBase
    {
        public override async Task<EmployeesSyncResponse> Sync(EmployeesSyncRequest request, ServerCallContext context)
        {
            var employees = await employeesHandler.LoadAsync(request.CompanyId, request.Timestamp.ToDateTime());

            var response = new EmployeesSyncResponse();
            if (employees.Any())
            {
                response.Employees.AddRange(employees.ConvertAll(e=> new EmployeesGrpc
                {
                    DivisionId = e.DivisionId,
                    EmployeesId = e.EmployeesId,
                    TotalQuantity = e.TotalQuantity,
                    SkillLevel = e.SkillLevel,
                    Salary = e.SalaryPerEmployee
                }));
                response.UpdatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc));
            }

            return response;
        }
    }
}
