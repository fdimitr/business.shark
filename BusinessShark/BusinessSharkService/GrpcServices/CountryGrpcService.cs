using BusinessSharkService.Handlers.Location;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices
{
    public class CountryGrpcService(
        ILogger<CountryGrpcService> logger, CountryHandler countryHandler) : CountryService.CountryServiceBase
    {
        public override async Task<CountrySyncResponse> Sync(CountrySyncRequest request, ServerCallContext context)
        {
            var countries = await countryHandler.LoadAsync(request.Timestamp.ToDateTime());
            var response = new CountrySyncResponse();
            if (countries.Any())
            {
                response.Countries.AddRange(countries.ConvertAll(c => new CountryGrpc
                {
                    CountryId = c.CountryId,
                    Name = c.Name,
                    Code = c.Code
                }));

                response.UpdatedAt =
                    Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(countries.Max(c => c.UpdatedAt));
            }

            return response;
        }
    }
}