using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class ProductCategoryEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public required string Name { get; set; }

        public int SortOrder { get; set; }
    }
}
