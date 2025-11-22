using BusinessSharkService.Handlers.Location;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices
{
    public class CityGrpcService(
        ILogger<CityGrpcService> logger, CityHandler cityHandler) : CityService.CityServiceBase
    {
        public override async Task<CitySyncResponse> Sync(CitySyncRequest request, ServerCallContext context)
        {
            var cities = await cityHandler.LoadAsync(request.Timestamp.ToDateTime());
            var response = new CitySyncResponse();
            if (cities.Any())
            {
                response.Cities.AddRange(cities.ConvertAll(c => new CityGrpc
                {
                    CityId = c.CityId,
                    Name = c.Name,
                    CountryId = c.CountryId,
                    Population = c.Population,
                    AverageSalary = c.AverageSalary,
                    BaseLandPrice = c.BaseLandPrice,
                    LandTax = c.LandTax,
                    WealthLevel = c.WealthLevel,
                    Happiness = c.Happiness

                }));

                response.UpdatedAt = Timestamp.FromDateTime(cities.Max(c => c.UpdatedAt));
            }

            return response;
        }
    }
}
