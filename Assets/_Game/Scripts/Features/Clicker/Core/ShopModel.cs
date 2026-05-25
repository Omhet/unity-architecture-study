using ObservableCollections;

namespace App.Clicker.Core
{
    public class ShopModel
    {
        public ObservableList<ShopItem> Items { get; } = new ObservableList<ShopItem>();
    }
}
