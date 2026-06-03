namespace App.Shop.Core
{
    using System;

    [Serializable]
    public struct ShopProgressionEntry
    {
        public int Level;
        public string[] ShopItemIds;
    }
}
