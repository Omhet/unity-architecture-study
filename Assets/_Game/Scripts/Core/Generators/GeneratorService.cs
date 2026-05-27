namespace App.Generators.Core
{
    using App.Resources.Core;

    public class GeneratorService
    {
        private readonly GeneratorModel _generatorModel;
        private readonly PlayerGeneratorModel _playerGeneratorModel;
        private readonly ResourceModel _resourceModel;

        public GeneratorService(
            GeneratorModel generatorModel,
            PlayerGeneratorModel playerGeneratorModel,
            ResourceModel resourceModel)
        {
            _generatorModel = generatorModel;
            _playerGeneratorModel = playerGeneratorModel;
            _resourceModel = resourceModel;
        }

        public bool TryGenerate(string generatorId)
        {
            if (!_playerGeneratorModel.IsOwned(generatorId))
            {
                return false;
            }

            if (!_generatorModel.TryGetById(generatorId, out var generator) || generator == null)
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