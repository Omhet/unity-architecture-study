using ObservableCollections;

namespace App.Shop.Core
{
    public class ShopModel
    {
        public ObservableList<ShopItem> Items { get; } = new ObservableList<ShopItem>();
    }
}
