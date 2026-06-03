namespace App.Shop.Core
{
    using System.Collections.Generic;

    public class ShopRegistry
    {
        private readonly List<ShopItemDefinition> _recipeItems = new List<ShopItemDefinition>();
        private readonly List<ShopItemDefinition> _generatorItems = new List<ShopItemDefinition>();

        public void Load(ShopCatalogConfig config)
        {
            _recipeItems.Clear();
            _generatorItems.Clear();

            if (config?.Items == null)
            {
                return;
            }

            if (config.Items.Recipes != null)
            {
                for (int i = 0; i < config.Items.Recipes.Length; i++)
                {
                    var item = config.Items.Recipes[i];
                    if (string.IsNullOrWhiteSpace(item.Id))
                    {
                        continue;
                    }

                    _recipeItems.Add(item);
                }
            }

            if (config.Items.Generators != null)
            {
                for (int i = 0; i < config.Items.Generators.Length; i++)
                {
                    var item = config.Items.Generators[i];
                    if (string.IsNullOrWhiteSpace(item.Id))
                    {
                        continue;
                    }

                    _generatorItems.Add(item);
                }
            }
        }

        public bool TryGetAny(string shopItemId, out ShopItemType itemType, out ShopItemDefinition definition)
        {
            itemType = default;
            definition = default;

            if (string.IsNullOrWhiteSpace(shopItemId))
            {
                return false;
            }

            for (int i = 0; i < _recipeItems.Count; i++)
            {
                var current = _recipeItems[i];
                if (current.Id == shopItemId)
                {
                    itemType = ShopItemType.Recipe;
                    definition = current;
                    return true;
                }
            }

            for (int i = 0; i < _generatorItems.Count; i++)
            {
                var current = _generatorItems[i];
                if (current.Id == shopItemId)
                {
                    itemType = ShopItemType.Generator;
                    definition = current;
                    return true;
                }
            }

            return false;
        }

        public int RecipeCount => _recipeItems.Count;
        public int GeneratorCount => _generatorItems.Count;

        public ShopItemDefinition this[ShopItemType type, int index]
        {
            get
            {
                if (type == ShopItemType.Recipe)
                    return _recipeItems[index];
                return _generatorItems[index];
            }
        }
    }
}
