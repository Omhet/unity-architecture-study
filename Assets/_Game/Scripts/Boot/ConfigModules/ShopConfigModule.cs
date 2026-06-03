namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Boot.Utility;
    using App.Generators.Core;
    using App.Recipes.Core;
    using App.Shop.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class ShopConfigModule : IConfigModule
    {
        private readonly ShopRegistry _shopRegistry;
        private readonly ShopProgressionRegistry _progressionRegistry;

        public string Key => "shop";

        public ShopConfigModule(ShopRegistry shopRegistry, ShopProgressionRegistry progressionRegistry)
        {
            _shopRegistry = shopRegistry;
            _progressionRegistry = progressionRegistry;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<ShopCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var shopConfig = bundle.GetConfig<ShopCatalogConfig>(Key);
            if (shopConfig?.Items == null)
            {
                errors.Add("Missing shop catalog.");
                return;
            }

            // Collect all shop item IDs for cross-reference validation
            var shopItemIds = new HashSet<string>();

            // Validate recipe shop items
            if (shopConfig.Items.Recipes != null)
            {
                ConfigValidationHelper.ValidateUniqueIds(shopConfig.Items.Recipes, x => x.Id, "recipe shop item", errors);

                foreach (var item in shopConfig.Items.Recipes)
                {
                    if (!string.IsNullOrWhiteSpace(item.Id))
                    {
                        shopItemIds.Add(item.Id);
                    }
                }
            }

            // Validate generator shop items and check for cross-array duplicates
            if (shopConfig.Items.Generators != null)
            {
                ConfigValidationHelper.ValidateUniqueIds(shopConfig.Items.Generators, x => x.Id, "generator shop item", errors);

                foreach (var item in shopConfig.Items.Generators)
                {
                    if (!string.IsNullOrWhiteSpace(item.Id))
                    {
                        if (!shopItemIds.Add(item.Id))
                        {
                            errors.Add("Duplicate shop item id across arrays: " + item.Id);
                        }
                    }
                }
            }

            // Validate recipe references against RecipeRegistry
            var recipes = bundle.GetConfig<RecipeCatalogConfig>("recipes");
            var recipeIds = ConfigValidationHelper.BuildIdSet(recipes?.Recipes, x => x?.Id);

            if (shopConfig.Items.Recipes != null)
            {
                foreach (var item in shopConfig.Items.Recipes)
                {
                    if (string.IsNullOrWhiteSpace(item.ItemId))
                        continue;

                    if (!recipeIds.Contains(item.ItemId))
                    {
                        errors.Add("Recipe shop item references unknown recipe: " + item.ItemId + " (shop item: " + item.Id + ")");
                    }
                }
            }

            // Validate generator references against GeneratorRegistry
            var generators = bundle.GetConfig<GeneratorCatalogConfig>("generators");
            var generatorIds = ConfigValidationHelper.BuildIdSet(generators?.Generators, x => x?.Id);

            if (shopConfig.Items.Generators != null)
            {
                foreach (var item in shopConfig.Items.Generators)
                {
                    if (string.IsNullOrWhiteSpace(item.ItemId))
                        continue;

                    if (!generatorIds.Contains(item.ItemId))
                    {
                        errors.Add("Generator shop item references unknown generator: " + item.ItemId + " (shop item: " + item.Id + ")");
                    }
                }
            }

            // Validate progression references against shop items
            if (shopConfig.Progression != null)
            {
                foreach (var entry in shopConfig.Progression)
                {
                    if (entry.ShopItemIds == null) continue;

                    foreach (var shopItemId in entry.ShopItemIds)
                    {
                        if (!string.IsNullOrWhiteSpace(shopItemId) && !shopItemIds.Contains(shopItemId))
                        {
                            errors.Add("Progression references unknown shop item: " + shopItemId + " (level: " + entry.Level + ")");
                        }
                    }
                }
            }
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<ShopCatalogConfig>(Key);
            _shopRegistry.Load(config);

            if (config?.Progression != null)
            {
                _progressionRegistry.Load(config.Progression);
            }
        }
    }
}
