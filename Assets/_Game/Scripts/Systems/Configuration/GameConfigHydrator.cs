namespace App.Systems.Configuration
{
    using App.GameConfig.Core;
    using App.Resources.Core;
    using App.Generators.Core;

    public class GameConfigHydrator
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly ResourceRegistry _resourceRegistry;
        private readonly ResourceState _resourceState;

        public GameConfigHydrator(
            GeneratorRegistry generatorRegistry,
            ResourceRegistry resourceRegistry,
            ResourceState resourceState
            )
        {
            _generatorRegistry = generatorRegistry;
            _resourceRegistry = resourceRegistry;
            _resourceState = resourceState;
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            HydrateGenerators(bundle.Generators);
            HydrateResources(bundle.Resources);
        }

        private void HydrateResources(ResourceCatalogConfig config)
        {
            _resourceRegistry.Load(config);

            // TODO: For now initialize resource state with all resources from config
            // We need to refine requirements, maybe resources will unlock as player progress or something similar, but for now we can just initialize everything to 0
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

        private void HydrateGenerators(GeneratorCatalogConfig config)
        {
            _generatorRegistry.Load(config);
        }
    }
}
