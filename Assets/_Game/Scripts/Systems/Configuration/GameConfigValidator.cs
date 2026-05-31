namespace App.Systems.Configuration
{
    using System;
    using System.Collections.Generic;
    using App.GameConfig.Core;

    public class GameConfigValidator
    {
        public void ValidateOrThrow(GameCatalogBundle bundle)
        {
            var errors = new List<string>();

            if (bundle == null)
            {
                errors.Add("Config bundle is null.");
                ThrowIfInvalid(errors);
                return;
            }

            ValidateResourceCatalog(bundle.Resources, errors);
            ValidateGeneratorCatalog(bundle.Generators, bundle.Resources, errors);
            ValidateProductCatalog(bundle.Products, errors);
            ValidateRecipeCatalog(bundle.Recipes, bundle.Resources, bundle.Products, errors);

            ThrowIfInvalid(errors);
        }

        private static void ValidateResourceCatalog(ResourceCatalogConfig config, List<string> errors)
        {
            if (config?.Resources == null)
            {
                errors.Add("Missing resources catalog.");
                return;
            }

            ValidateUniqueIds(config.Resources, x => x?.Id, "resource", errors);
        }

        private static void ValidateGeneratorCatalog(GeneratorCatalogConfig generators, ResourceCatalogConfig resources, List<string> errors)
        {
            if (generators?.Generators == null)
            {
                errors.Add("Missing generators catalog.");
                return;
            }

            ValidateUniqueIds(generators.Generators, x => x?.Id, "generator", errors);
            var resourceIds = BuildIdSet(resources?.Resources, x => x?.Id);
            foreach (var generator in generators.Generators)
            {
                if (generator == null)
                {
                    continue;
                }

                if (!resourceIds.Contains(generator.ResourceId))
                {
                    errors.Add("Generator references unknown resource: " + generator.ResourceId + " (generator: " + generator.Id + ")");
                }
            }
        }

        private static void ValidateProductCatalog(ProductCatalogConfig config, List<string> errors)
        {
            if (config?.Products == null)
            {
                errors.Add("Missing products catalog.");
                return;
            }

            ValidateUniqueIds(config.Products, x => x?.Id, "product", errors);
        }

        private void ValidateRecipeCatalog(RecipeCatalogConfig recipes, ResourceCatalogConfig resources, ProductCatalogConfig products, List<string> errors)
        {
            if (recipes?.Recipes == null)
            {
                errors.Add("Missing recipes catalog.");
                return;
            }

            ValidateUniqueIds(recipes.Recipes, x => x?.Id, "recipe", errors);
            var resourceIds = BuildIdSet(resources?.Resources, x => x?.Id);
            var productIds = BuildIdSet(products?.Products, x => x?.Id);
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

                if (recipe.InputResourceIds != null)
                {
                    foreach (var inputResourceId in recipe.InputResourceIds.Keys)
                    {
                        if (!resourceIds.Contains(inputResourceId))
                        {
                            errors.Add("Recipe references unknown input resource: " + inputResourceId + " (recipe: " + recipe.Id + ")");
                        }
                    }
                }
            }
        }


        private static void ValidateUniqueIds<T>(IEnumerable<T> items, Func<T, string> selector, string itemType, List<string> errors)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                string id = selector(item);
                if (string.IsNullOrWhiteSpace(id))
                {
                    errors.Add("Found " + itemType + " with missing id.");
                    continue;
                }

                if (!ids.Add(id))
                {
                    errors.Add("Duplicate " + itemType + " id: " + id);
                }
            }
        }

        private static HashSet<string> BuildIdSet<T>(IEnumerable<T> items, Func<T, string> selector)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (items == null)
            {
                return ids;
            }

            foreach (var item in items)
            {
                string id = selector(item);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        private static void ThrowIfInvalid(List<string> errors)
        {
            if (errors.Count == 0)
            {
                return;
            }

            throw new InvalidOperationException("Game config validation failed:\n- " + string.Join("\n- ", errors));
        }
    }
}
