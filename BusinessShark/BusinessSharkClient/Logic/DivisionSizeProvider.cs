using BusinessSharkService;
using BusinessSharkShared;

namespace BusinessSharkClient.Logic
{
    public class DivisionSizeProvider(DivisionSizeService.DivisionSizeServiceClient serviceClient)
    {
        public async Task<List<DivisionSizeGrpc>> GetDivisionSizesAsync(DivisionType divisionTypeId)
        {
            var response = await serviceClient.LoadAsync(
                new DivisionSizeRequest { DivisionTypeId = (int)divisionTypeId });

            return response.Sizes.ToList();
        }
    }
}
