namespace App.Shop.Core
{
    using System;

    [Serializable]
    public class ShopCatalogConfig
    {
        public ShopItems Items;
        public ShopProgressionEntry[] Progression;

        [Serializable]
        public class ShopItems
        {
            public ShopItemDefinition[] Recipes;
            public ShopItemDefinition[] Generators;
        }
    }
}
