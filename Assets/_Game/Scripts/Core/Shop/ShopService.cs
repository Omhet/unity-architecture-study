namespace App.Shop.Core
{
    using App.Economy.Core;
    using App.Generators.Core;
    using App.Recipes.Core;

    public class ShopService
    {
        private readonly ShopRegistry _shopRegistry;
        private readonly ShopState _shopState;
        private readonly ShopProgressionRegistry _progressionRegistry;
        private readonly EconomyService _economyService;
        private readonly RecipeState _recipeState;
        private readonly GeneratorState _generatorState;

        public ShopService(
            ShopRegistry shopRegistry,
            ShopState shopState,
            ShopProgressionRegistry progressionRegistry,
            EconomyService economyService,
            RecipeState recipeState,
            GeneratorState generatorState)
        {
            _shopRegistry = shopRegistry;
            _shopState = shopState;
            _progressionRegistry = progressionRegistry;
            _economyService = economyService;
            _recipeState = recipeState;
            _generatorState = generatorState;
        }

        public void RefreshAvailability(int playerLevel)
        {
            var unlockedIds = _progressionRegistry.GetUnlockedUpToLevel(playerLevel);

            for (int i = 0; i < unlockedIds.Count; i++)
            {
                // Check if shopr already contains this item (in case multiple progression entries unlock the same item)
                if (!_shopState.AvailableShopItemIds.Contains(unlockedIds[i]))
                {
                    _shopState.AvailableShopItemIds.Add(unlockedIds[i]);
                }
            }
        }

        public bool TryToBuy(string shopItemId)
        {
            // First check if the item is actually available for purchase in the game
            if (!_shopRegistry.TryGetAny(shopItemId, out var itemType, out var definition))
            {
                return false;
            }

            // Check if player already owns the item
            switch (itemType)
            {
                case ShopItemType.Recipe:
                    if (_recipeState.PlayerOwnedRecipeIds.Contains(definition.ItemId))
                    {
                        return false;
                    }
                    break;
                case ShopItemType.Generator:
                    if (_generatorState.PlayerOwnedGeneratorIds.Contains(definition.ItemId))
                    {
                        return false;
                    }
                    break;
            }

            // Then check if the player has enough currency to buy the item
            if (!_economyService.TrySpend(definition.Price))
            {
                return false;
            }

            switch (itemType)
            {
                case ShopItemType.Recipe:
                    _recipeState.PlayerOwnedRecipeIds.Add(definition.ItemId);
                    break;
                case ShopItemType.Generator:
                    _generatorState.PlayerOwnedGeneratorIds.Add(definition.ItemId);
                    break;
            }

            return true;
        }
    }
}
