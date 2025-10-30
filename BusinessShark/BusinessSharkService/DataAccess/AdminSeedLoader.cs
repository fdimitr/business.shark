using Newtonsoft.Json;
using BusinessSharkService.DataAccess.Models.Player;
using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.DataAccess.Models;

public class AdminSeedLoader
{
    public class AdminSeedData
    {
        public List<Player> Players { get; set; }
        public List<Company> Companies { get; set; }
        public List<Sawmill> Sawmills { get; set; }
        public List<Warehouse> Warehouses { get; set; }
        public List<Tools> Tools { get; set; }
        public List<Employees> Employees { get; set; }
    }

    public static AdminSeedData LoadAdminSeedData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file {filePath} does not exist.");
        }

        string json = File.ReadAllText(filePath);
        var adminSeedData = JsonConvert.DeserializeObject<AdminSeedData>(json);

        if (adminSeedData == null)
        {
            throw new InvalidOperationException("Failed to deserialize the JSON data.");
        }

        return adminSeedData;
    }
}