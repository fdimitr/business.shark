namespace BusinessSharkClient.Logic.Models
{
    public class SawmillListModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Volume { get; set; }
        public required string ProductName { get; set; }
        public ImageSource? ProductIcon { get; set; }
    }
}
