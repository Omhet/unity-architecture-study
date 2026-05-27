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
            ValidateProductCatalog(bundle.Products, errors);
            ValidateGeneratorCatalog(bundle.Generators, bundle.Resources, errors);
            ValidateRecipeCatalog(bundle.Recipes, bundle.Resources, bundle.Products, errors);
            ValidateOrderCatalog(bundle.Orders, bundle.Products, errors);
            ValidateProgressionCatalog(bundle.Progression, errors);
            ValidateEconomyCatalog(bundle.Economy, errors);
            ValidateTalentCatalog(bundle.Talents, errors);
            ValidateShopCatalog(bundle.Shop, errors);

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

        private static void ValidateProductCatalog(ProductCatalogConfig config, List<string> errors)
        {
            if (config?.Products == null)
            {
                errors.Add("Missing products catalog.");
                return;
            }

            ValidateUniqueIds(config.Products, x => x?.Id, "product", errors);
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

        private static void ValidateRecipeCatalog(RecipeCatalogConfig recipes, ResourceCatalogConfig resources, ProductCatalogConfig products, List<string> errors)
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

                if (recipe.ResourceInputs == null || recipe.ResourceInputs.Length == 0)
                {
                    errors.Add("Recipe has no inputs: " + recipe.Id);
                }

                if (recipe.ProductOutput == null || string.IsNullOrWhiteSpace(recipe.ProductOutput.ProductId))
                {
                    errors.Add("Recipe has invalid output: " + recipe.Id);
                }
                else if (!productIds.Contains(recipe.ProductOutput.ProductId))
                {
                    errors.Add("Recipe output references unknown product: " + recipe.ProductOutput.ProductId + " (recipe: " + recipe.Id + ")");
                }

                if (recipe.ResourceInputs == null)
                {
                    continue;
                }

                foreach (var input in recipe.ResourceInputs)
                {
                    if (input == null)
                    {
                        continue;
                    }

                    if (!resourceIds.Contains(input.ResourceId))
                    {
                        errors.Add("Recipe input references unknown resource: " + input.ResourceId + " (recipe: " + recipe.Id + ")");
                    }
                }
            }
        }

        private static void ValidateOrderCatalog(OrderCatalogConfig orders, ProductCatalogConfig products, List<string> errors)
        {
            if (orders?.Orders == null)
            {
                errors.Add("Missing orders catalog.");
                return;
            }

            ValidateUniqueIds(orders.Orders, x => x?.Id, "order", errors);
            var productIds = BuildIdSet(products?.Products, x => x?.Id);

            foreach (var order in orders.Orders)
            {
                if (order?.ProductRequirements == null)
                {
                    continue;
                }

                foreach (var requirement in order.ProductRequirements)
                {
                    if (requirement == null)
                    {
                        continue;
                    }

                    if (!productIds.Contains(requirement.ProductId))
                    {
                        errors.Add("Order requirement references unknown product: " + requirement.ProductId + " (order: " + order.Id + ")");
                    }
                }
            }
        }

        private static void ValidateProgressionCatalog(ProgressionCatalogConfig config, List<string> errors)
        {
            if (config?.Levels == null)
            {
                errors.Add("Missing progression catalog.");
                return;
            }

            if (config.StartingLevel < 1)
            {
                errors.Add("StartingLevel must be >= 1.");
            }

            if (config.StartingExperience < 0)
            {
                errors.Add("StartingExperience must be >= 0.");
            }

            if (config.StartingTalentPoints < 0)
            {
                errors.Add("StartingTalentPoints must be >= 0.");
            }

            ValidateUniqueIds(config.Levels, x => x == null ? string.Empty : x.Level.ToString(), "level", errors);
        }

        private static void ValidateTalentCatalog(TalentCatalogConfig config, List<string> errors)
        {
            if (config?.Talents == null)
            {
                errors.Add("Missing talents catalog.");
                return;
            }

            ValidateUniqueIds(config.Talents, x => x?.Id, "talent", errors);
        }

        private static void ValidateEconomyCatalog(EconomyCatalogConfig config, List<string> errors)
        {
            if (config == null)
            {
                errors.Add("Missing economy catalog.");
                return;
            }

            if (config.StartingMoney < 0)
            {
                errors.Add("StartingMoney must be >= 0.");
            }
        }

        private static void ValidateShopCatalog(App.Shop.Core.ShopConfig config, List<string> errors)
        {
            if (config?.AvailableItems == null)
            {
                errors.Add("Missing shop catalog.");
                return;
            }

            ValidateUniqueIds(config.AvailableItems, x => x?.ItemId, "shop item", errors);
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
