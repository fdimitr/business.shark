using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Items
{
    [Comment("They represent product located in warehouses and participating in production")]
    public class Product : ICloneable
    {
        [Key] 
        public int ProductId { get; set; }

        public int ProductDefinitionId { get; set; }
        public ProductDefinition? ProductDefinition { get; set; }

        [Comment("Stored quality")]
        public double Quality { get; set; }

        [Comment("Stored quantity")]
        public int Quantity { get; set; }

        [Comment("Price of the product in storage")]
        public double Price { get; set; }

        [Comment("Current price of the product in production")]
        public double ProcessingPrice { get; set; }

        [Comment("Current quality of the product in production")]
        public double ProcessingQuality { get; set; }

        [Comment("Current quantity of the product in production")]
        public int ProcessingQuantity { get; set; }

        public void ResetProcessing()
        {
            ProcessingQuality = 0;
            ProcessingQuantity = 0;
            ProcessingPrice = 0;
        }

        public object Clone()
        {
            return new Product
            {
                ProductDefinitionId = this.ProductDefinitionId,
                ProductDefinition = this.ProductDefinition,
                Quality = this.Quality,
                Price = this.Price
            };
        }
    }
}
