namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Boot.Utility;
    using App.Products.Core;
    using App.Recipes.Core;
    using App.Resources.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class RecipeConfigModule : IConfigModule
    {
        private readonly RecipeRegistry _recipeRegistry;
        private readonly RecipeState _recipeState;

        public string Key => "recipes";

        public RecipeConfigModule(RecipeRegistry recipeRegistry, RecipeState recipeState)
        {
            _recipeRegistry = recipeRegistry;
            _recipeState = recipeState;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<RecipeCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var recipes = bundle.GetConfig<RecipeCatalogConfig>(Key);
            if (recipes?.Recipes == null)
            {
                errors.Add("Missing recipes catalog.");
                return;
            }

            ConfigValidationHelper.ValidateUniqueIds(recipes.Recipes, x => x?.Id, "recipe", errors);

            var resources = bundle.GetConfig<ResourceCatalogConfig>("resources");
            var products = bundle.GetConfig<ProductCatalogConfig>("products");
            var resourceIds = ConfigValidationHelper.BuildIdSet(resources?.Resources, x => x?.Id);
            var productIds = ConfigValidationHelper.BuildIdSet(products?.Products, x => x?.Id);

            foreach (var recipe in recipes.Recipes)
            {
                if (recipe == null)
                {
                    continue;
                }

                if (!productIds.Contains(recipe.OutputProductId))
                {
                    errors.Add("Recipe references unknown output product: " + recipe.OutputProductId + " (recipe: " + recipe.Id + ")");
                }

                if (recipe.InputResources != null)
                {
                    foreach (var inputResourceId in recipe.InputResources.Keys)
                    {
                        if (!resourceIds.Contains(inputResourceId))
                        {
                            errors.Add("Recipe references unknown input resource: " + inputResourceId + " (recipe: " + recipe.Id + ")");
                        }
                        else if (recipe.InputResources[inputResourceId] <= 0)
                        {
                            errors.Add("Recipe has non-positive amount for input resource: " + inputResourceId + " (recipe: " + recipe.Id + ")");
                        }
                    }
                }
                else
                {
                    errors.Add("Recipe is missing input resources: " + recipe.Id);
                }
            }
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<RecipeCatalogConfig>(Key);
            _recipeRegistry.Load(config);

            if (config?.Recipes != null && config.Recipes.Length > 0)
            {
                var firstRecipe = config.Recipes[0];
                if (firstRecipe != null && !string.IsNullOrWhiteSpace(firstRecipe.Id))
                {
                    _recipeState.PlayerOwnedRecipeIds.Add(firstRecipe.Id);
                }
            }
        }
    }
}
