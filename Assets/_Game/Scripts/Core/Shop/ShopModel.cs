using ObservableCollections;

namespace App.Shop.Core
{
    public class ShopModel
    {
        public ObservableList<ShopItemState> Items { get; } = new ObservableList<ShopItemState>();

        public bool TryGetById(string itemId, out ShopItemState item)
        {
            item = null;
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            for (int i = 0; i < Items.Count; i++)
            {
                var current = Items[i];
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
