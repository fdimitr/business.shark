using System.ComponentModel.DataAnnotations;

namespace BusinessSharkClient.Data.Entities
{
    public class DataState
    {
        [Key]
        public required string Key { get; set; }
        public required DateTime Value { get; set; }
    }
}
