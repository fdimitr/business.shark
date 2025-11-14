using BusinessSharkService;
using BusinessSharkShared;

namespace BusinessSharkClient.Logic
{
    public class DivisionSizeProvider
    {
        private DivisionSizeService.DivisionSizeServiceClient _serviceClient;
        public DivisionSizeProvider(DivisionSizeService.DivisionSizeServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task<List<DivisionSizeGrpc>> GetDivisionSizesAsync(DivisionType divisionTypeId)
        {
            var response = await _serviceClient.LoadAsync(
                new DivisionSizeRequest { DivisionTypeId = (int)divisionTypeId });

            return response.Sizes.ToList();
        }
    }
}
