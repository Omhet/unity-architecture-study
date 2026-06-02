namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Boot.Utility;
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

            ConfigValidationHelper.ValidateUniqueIds(config.Resources, x => x?.Id, "resource", errors);
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
    }
}
