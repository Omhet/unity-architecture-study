namespace App.Shop.Core
{
    using System.Collections.Generic;

    public class ShopRegistry
    {
        private readonly List<ShopDefinition> _items = new List<ShopDefinition>();

        public void Load(ShopConfig config)
        {
            _items.Clear();

            if (config?.AvailableItems == null)
            {
                return;
            }

            for (int i = 0; i < config.AvailableItems.Length; i++)
            {
                var item = config.AvailableItems[i];
                if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
                {
                    continue;
                }

                _items.Add(item);
            }
        }

        public bool TryGetById(string itemId, out ShopDefinition item)
        {
            item = null;
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                var current = _items[i];
                if (current != null && current.ItemId == itemId)
                {
                    item = current;
                    return true;
                }
            }

            return false;
        }
    }
}