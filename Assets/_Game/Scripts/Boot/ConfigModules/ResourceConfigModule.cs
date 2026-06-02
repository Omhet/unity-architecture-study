namespace App.Boot.ConfigModules
{
    using System;
    using System.Collections.Generic;
    using App.Resources.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class ResourceConfigModule : IConfigModule
    {
        private readonly ResourceRegistry _resourceRegistry;
        private readonly ResourceState _resourceState;

        public string Key => "resources";

        public ResourceConfigModule(ResourceRegistry resourceRegistry, ResourceState resourceState)
        {
            _resourceRegistry = resourceRegistry;
            _resourceState = resourceState;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<ResourceCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var config = bundle.GetConfig<ResourceCatalogConfig>(Key);
            if (config?.Resources == null)
            {
                errors.Add("Missing resources catalog.");
                return;
            }

            ValidateUniqueIds(config.Resources, x => x?.Id, "resource", errors);
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<ResourceCatalogConfig>(Key);
            _resourceRegistry.Load(config);

            if (config?.Resources != null)
            {
                for (int i = 0; i < config.Resources.Length; i++)
                {
                    var resource = config.Resources[i];
                    if (resource != null && !string.IsNullOrWhiteSpace(resource.Id))
                    {
                        _resourceState.Balances[resource.Id] = 0;
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
    }
}
