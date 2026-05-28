namespace App.Shop.Core
{
    [System.Serializable]
    public class ShopConfig
    {
        public ShopDefinition[] AvailableItems;
    }

    [System.Serializable]
    public class ShopDefinition
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
    }

    [System.Serializable]
    public class ShopItemState
    {
        public string ItemId;

        public bool IsVisible;
        public bool IsBuyable;
        public bool IsPurchased;
    }
}
