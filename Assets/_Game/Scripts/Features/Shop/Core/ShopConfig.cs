namespace App.Shop.Core
{
    // Pure DTO for Addressables/JSON parsing
    [System.Serializable]
    public class ShopConfig
    {
        public ShopItem[] AvailableItems;
    }

    [System.Serializable]
    public class ShopItem
    {
        public string ItemId;
        public string DisplayName;
        public int Cost;
    }
}
