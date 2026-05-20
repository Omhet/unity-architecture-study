using ObservableCollections;

namespace Core.Shop
{
    public class ShopModel
    {
        public ObservableList<ShopItem> Items { get; } = new ObservableList<ShopItem>();
    }
}
