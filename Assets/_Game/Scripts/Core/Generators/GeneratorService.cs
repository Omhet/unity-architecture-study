namespace App.Generators.Core
{
    using App.Resources.Core;

    public class GeneratorService
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly PlayerGeneratorModel _playerGeneratorModel;
        private readonly ResourceModel _resourceModel;

        public GeneratorService(
            GeneratorRegistry generatorRegistry,
            PlayerGeneratorModel playerGeneratorModel,
            ResourceModel resourceModel)
        {
            _generatorRegistry = generatorRegistry;
            _playerGeneratorModel = playerGeneratorModel;
            _resourceModel = resourceModel;
        }

        public bool TryGenerate(string generatorId)
        {
            if (!_playerGeneratorModel.IsOwned(generatorId))
            {
                return false;
            }

            if (!_generatorRegistry.TryGetById(generatorId, out var generator) || generator == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(generator.ResourceId) || generator.AmountPerClick <= 0)
            {
                return false;
            }

            _resourceModel.AddAmount(generator.ResourceId, generator.AmountPerClick);
            return true;
        }
    }
}