using System.Collections.ObjectModel;
using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Logic.Models;

namespace BusinessSharkClient.Logic
{
    public class SawmillProvider(
        ILocalRepository<SawmillEntity> repo,
        GlobalDataProvider globalDataProvider)
    {
        public async Task<ObservableCollection<SawmillListGroup>> LoadList(int companyId)
        {
            var response = await repo.GetAllAsync();
            if (!response.Any())
                return new ObservableCollection<SawmillListGroup>();

            // Cache product definitions for fast lookup
            var productLookup = globalDataProvider.ProductDefinitions
                .ToDictionary(p => p.ProductDefinitionId);

            var grouped = response
                .GroupBy(s => new { s.City, s.CountryCode })
                .Select(group =>
                {
                    var sawmills = group.Select(s =>
                    {
                        productLookup.TryGetValue(s.ProductDefinitionId, out var productDef);

                        return new SawmillListModel
                        {
                            Id = s.Id,
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

        public async Task<SawmillEntity> LoadDetail(int divisionId)
        {
            var response = await repo.GetByIdAsync(divisionId);
            if (response == null) throw new Exception($"Sawmill wasn't found by id = {divisionId}");
            return response;
        }
    }
}
