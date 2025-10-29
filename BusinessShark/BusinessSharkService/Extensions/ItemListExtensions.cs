using BusinessSharkService.DataAccess.Models.Items;

namespace BusinessSharkService.Extensions
{
    public static class ItemListExtensions
    {
        /// <summary>
        /// Tries to get an Item from the list by its ItemId.
        /// Returns true if found; otherwise false. The found item is placed into the out parameter.
        /// </summary>
        public static bool TryGetItem(this List<WarehouseProduct>? items, int itemDefinitionId, out WarehouseProduct item)
        {
            ArgumentNullException.ThrowIfNull(items);

            // Simple linear scan; avoids LINQ allocation and permits early exit.
            foreach (var current in items)
            {
                if (current.ProductDefinitionId == itemDefinitionId)
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