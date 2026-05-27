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

        public bool InitialVisibility;
        public bool InitialBuyability;
        public int RequiredLevel;
        public string[] PrerequisiteUnlockIds;
        public bool OneTimePurchase;
        public string UnlockTargetType;
        public string UnlockTargetId;

        public bool IsVisible;
        public bool IsBuyable;
    }
}
