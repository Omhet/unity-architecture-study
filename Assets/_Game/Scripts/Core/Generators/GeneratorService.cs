namespace App.Generators.Core
{
    using App.Resources.Core;

    public class GeneratorService
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly GeneratorState _generatorState;
        private readonly ResourceState _resourceState;

        public GeneratorService(
            GeneratorRegistry generatorRegistry,
            GeneratorState generatorState,
            ResourceState resourceState)
        {
            _generatorRegistry = generatorRegistry;
            _generatorState = generatorState;
            _resourceState = resourceState;
        }

        public bool TryGenerate(string generatorId)
        {
            if (!_generatorState.IsOwned(generatorId))
            {
                return false;
            }

            if (!_generatorRegistry.TryGetById(generatorId, out var generator) || generator == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(generator.ResourceId))
            {
                return false;
            }

            // TODO: Hardcode to 1 for now, but eventually this should be based on generator level or something similar like player progression
            _resourceState.AddAmount(generator.ResourceId, 1);

            return true;
        }
    }
}