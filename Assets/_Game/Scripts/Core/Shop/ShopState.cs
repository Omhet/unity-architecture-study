namespace App.Shop.Core
{
    using ObservableCollections;

    public class ShopState
    {
        public ObservableList<string> AvailableShopItemIds { get; } = new ObservableList<string>();
    }
}
