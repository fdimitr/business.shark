using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Items
{
    public class ProductCategory
    {
        [Key]
        public int ProductCategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public int SortOrder { get; set; }
    }
}
