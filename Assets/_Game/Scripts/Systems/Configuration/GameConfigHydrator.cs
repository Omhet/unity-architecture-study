namespace App.Systems.Configuration
{
    using App.GameConfig.Core;
    using App.Resources.Core;
    using App.Generators.Core;

    public class GameConfigHydrator
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly ResourceModel _resourceModel;

        public GameConfigHydrator(
            GeneratorRegistry generatorRegistry,
            ResourceModel resourceModel
            )
        {
            _resourceModel = resourceModel;
            _generatorRegistry = generatorRegistry;
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            HydrateGenerators(bundle.Generators);
            HydrateResources(bundle.Resources);
        }

        private void HydrateResources(ResourceCatalogConfig config)
        {

        }

        private void HydrateGenerators(GeneratorCatalogConfig config)
        {
            _generatorRegistry.Load(config);
        }
    }
}
