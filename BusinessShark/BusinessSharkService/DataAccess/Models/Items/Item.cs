using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Items
{
    public class Item : ICloneable
    {
        [Key] public int ItemId { get; set; }

        public int ItemDefinitionId { get; set; }
        public ItemDefinition? ItemDefinition { get; set; }
        public double Quality { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public double ProcessingPrice { get; set; }
        public double ProcessingQuality { get; set; }
        public int ProcessingQuantity { get; set; }

        public void ResetProcessing()
        {
            ProcessingQuality = 0;
            ProcessingQuantity = 0;
            ProcessingPrice = 0;
        }

        public object Clone()
        {
            return new Item
            {
                ItemDefinitionId = this.ItemDefinitionId,
                ItemDefinition = this.ItemDefinition,
                Quality = this.Quality,
                Price = this.Price
            };
        }
    }
}
