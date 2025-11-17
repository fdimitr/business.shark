using System.Collections.ObjectModel;
using BusinessSharkClient.Logic.Models;
using BusinessSharkService;

namespace BusinessSharkClient.Logic
{
    public class SawmillProvider
    {

        private readonly SawmillService.SawmillServiceClient _sawmillClient;
        private readonly GlobalDataProvider _globalDataProvider;

        public SawmillProvider(SawmillService.SawmillServiceClient sawmillClient, GlobalDataProvider globalDataProvider)
        {
            _sawmillClient = sawmillClient;
            _globalDataProvider = globalDataProvider;
        }

        public async Task<ObservableCollection<SawmillListGroup>> LoadList(int companyId)
        {
            var response = await _sawmillClient.LoadListAsync(new SawmillListRequest { CompanyId = companyId });
            if (response?.Sawmills == null || response.Sawmills.Count == 0)
                return new ObservableCollection<SawmillListGroup>();

            // Cache product definitions for fast lookup
            var productLookup = _globalDataProvider.ProductDefinitions
                .ToDictionary(p => p.ProductDefinitionId);

            var grouped = response.Sawmills
                .GroupBy(s => new { s.City, s.CountryCode })
                .Select(group =>
                {
                    var sawmills = group.Select(s =>
                    {
                        productLookup.TryGetValue(s.ProductDefinitionId, out var productDef);

                        return new SawmillListModel
                        {
                            Id = s.DivisionId,
                            Name = s.Name,
                            ProductName = productDef?.Name ?? "Unknown",
                            ProductIcon = productDef?.Image ?? string.Empty,
                            Volume = $"{s.VolumeCapacity}m³"
                        };
                    });

                    return new SawmillListGroup(
                        group.Key.City,
                        $"{group.Key.CountryCode.ToLowerInvariant()}.png",
                        sawmills
                    );
                });

            return new ObservableCollection<SawmillListGroup>(grouped);

        }

        public async Task<SawmillResponse> LoadDetail(int divisionId)
        {
            var response = await _sawmillClient.LoadDetailAsync(new SawmillRequest { DivisionId = divisionId });
            if (response == null) throw new Exception($"Sawmill wasn't found by id = {divisionId}");
            return response;
        }
    }
}
