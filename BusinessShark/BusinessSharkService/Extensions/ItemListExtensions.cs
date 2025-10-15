using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkService.Extensions
{
    public static class ItemListExtensions
    {
        /// <summary>
        /// Tries to get an Item from the list by its ItemId.
        /// Returns true if found; otherwise false. The found item is placed into the out parameter.
        /// </summary>
        public static bool TryGetItem(this List<Product> items, int itemId, out Product item)
        {
            ArgumentNullException.ThrowIfNull(items);

            // Simple linear scan; avoids LINQ allocation and permits early exit.
            for (var i = 0; i < items.Count; i++)
            {
                var current = items[i];
                if (current.ProductId == itemId)
                {
                    item = current;
                    return true;
                }
            }

            item = null!;
            return false;
        }
    }
}