namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Boot.Utility;
    using App.Generators.Core;
    using App.Resources.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class GeneratorConfigModule : IConfigModule
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly GeneratorState _generatorState;

        public string Key => "generators";

        public GeneratorConfigModule(GeneratorRegistry generatorRegistry, GeneratorState generatorState)
        {
            _generatorRegistry = generatorRegistry;
            _generatorState = generatorState;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<GeneratorCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var generators = bundle.GetConfig<GeneratorCatalogConfig>(Key);
            if (generators?.Generators == null)
            {
                errors.Add("Missing generators catalog.");
                return;
            }

            ConfigValidationHelper.ValidateUniqueIds(generators.Generators, x => x?.Id, "generator", errors);

            var resources = bundle.GetConfig<ResourceCatalogConfig>("resources");
            var resourceIds = ConfigValidationHelper.BuildIdSet(resources?.Resources, x => x?.Id);

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

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<GeneratorCatalogConfig>(Key);
            _generatorRegistry.Load(config);

            if (config?.Generators != null && config.Generators.Length > 0)
            {
                var firstGenerator = config.Generators[0];
                if (firstGenerator != null && !string.IsNullOrWhiteSpace(firstGenerator.Id))
                {
                    _generatorState.PlayerOwnedGeneratorIds.Add(firstGenerator.Id);
                }
            }
        }
    }
}
