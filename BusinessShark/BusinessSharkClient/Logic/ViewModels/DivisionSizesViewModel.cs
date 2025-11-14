using BusinessSharkClient.Logic.Models;
using BusinessSharkShared;

namespace BusinessSharkClient.Logic.ViewModels
{
    public class DivisionSizeViewModel
    {
        private readonly DivisionSizeProvider _divisionSizeProvider;

        public DivisionSizeViewModel(DivisionSizeProvider divisionSizeProvider)
        {
            _divisionSizeProvider = divisionSizeProvider;
        }

        public List<DivisionSizeModel> Sizes { get; set; } = new();

        public async Task LoadAsync(DivisionType divisionTypeId)
        {
            var sizes = await _divisionSizeProvider.GetDivisionSizesAsync(divisionTypeId);
            Sizes.AddRange(sizes.ConvertAll(size => new DivisionSizeModel
            {
                DivisionSizeId = size.DivisionSizeId,
                Size = size.Size,
                ConstructionCost = size.ConstructionCost,
                MaxEmployees = size.MaxEmployees,
                MaxTools = size.MaxTools,
                WarehouseVolume = size.WarehouseVolume
            }));
        }
    }
}
