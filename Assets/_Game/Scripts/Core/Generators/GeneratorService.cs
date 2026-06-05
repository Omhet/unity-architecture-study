namespace App.Generators.Core
{
    using System;
    using App.Resources.Core;
    using App.Talents.Core;

    public class GeneratorService
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly GeneratorState _generatorState;
        private readonly ResourceState _resourceState;
        private readonly TalentService _talentService;

        public GeneratorService(
            GeneratorRegistry generatorRegistry,
            GeneratorState generatorState,
            ResourceState resourceState,
            TalentService talentService)
        {
            _generatorRegistry = generatorRegistry;
            _generatorState = generatorState;
            _resourceState = resourceState;
            _talentService = talentService;
        }

        public bool TryGenerate(string generatorId)
        {
            if (!_generatorState.IsPlayerOwned(generatorId))
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

            int baseAmount = 1;
            float multiplier = _talentService.GetMultiplier("generator_boost");
            int amount = (int)Math.Ceiling(baseAmount * multiplier);
            _resourceState.AddAmount(generator.ResourceId, amount);

            return true;
        }
    }
}