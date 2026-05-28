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
