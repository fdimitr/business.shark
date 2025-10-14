namespace BusinessSharkService.DataAccess.Models
{
    public class Tools
    {
        public int TotalQuantity { get; set; }
        public double TechLevel { get; set; } = 1;
        public double Deprecation { get; set; }

        public int ActiveQuantity => (int)Math.Round(TotalQuantity * Deprecation);

    }
}
