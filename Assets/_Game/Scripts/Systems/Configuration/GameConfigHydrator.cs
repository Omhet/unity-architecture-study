namespace App.Systems.Configuration
{
    using App.GameConfig.Core;
    using App.Resources.Core;
    using App.Generators.Core;

    public class GameConfigHydrator
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly GeneratorState _generatorState;
        private readonly ResourceRegistry _resourceRegistry;
        private readonly ProductRegistry _productRegistry;
        private readonly ResourceState _resourceState;

        public GameConfigHydrator(
            GeneratorRegistry generatorRegistry,
            GeneratorState generatorState,
            ResourceRegistry resourceRegistry,
            ResourceState resourceState,
            ProductRegistry productRegistry
            )
        {
            _generatorRegistry = generatorRegistry;
            _generatorState = generatorState;
            _resourceRegistry = resourceRegistry;
            _resourceState = resourceState;
            _productRegistry = productRegistry;
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            HydrateGenerators(bundle.Generators);
            HydrateResources(bundle.Resources);
            HydrateProducts(bundle.Products);
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

            // TODO: For now initialize generator state with the first generator from config, 
            // but we need to refine requirements for this, maybe it should happen after onboarding or something similar, 
            // but for now we can just give player access to the first generator in the list
            if (config?.Generators != null && config.Generators.Length > 0)
            {
                var firstGenerator = config.Generators[0];
                if (firstGenerator != null && !string.IsNullOrWhiteSpace(firstGenerator.Id))
                {
                    _generatorState.PlayerOwnedGeneratorIds.Add(firstGenerator.Id);
                }
            }
        }

        private void HydrateProducts(ProductCatalogConfig config)
        {
            _productRegistry.Load(config);

            // For now we don't have any state to initialize for products, 
            // but we can add it here in the future if we decide to have some initial products available to the player
        }
    }
}
