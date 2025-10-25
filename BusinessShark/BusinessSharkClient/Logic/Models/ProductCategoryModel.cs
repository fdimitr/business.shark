namespace BusinessSharkClient.Logic.Models
{
    public class ProductCategoryModel
    {
        public int ProductCategoryId { get; set; }
        public required string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
