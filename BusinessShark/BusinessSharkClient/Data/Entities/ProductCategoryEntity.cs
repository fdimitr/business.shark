using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class ProductCategoryEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public int SortOrder { get; set; }

        public object[] GetKeyValues() => [Id];
    }
}
