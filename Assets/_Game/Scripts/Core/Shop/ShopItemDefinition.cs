namespace App.Shop.Core
{
    using System;

    [Serializable]
    public struct ShopItemDefinition
    {
        public string Id;
        public string ItemId;
        public int Price;
    }
}
