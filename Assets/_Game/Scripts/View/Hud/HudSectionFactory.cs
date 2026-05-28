namespace App.Hud.View
{
    using App.Generators.Core;
    using App.View;
    using VitalRouter;

    public class HudSectionFactory
    {
        private readonly GeneratorRegistry _generatorRegistry;
        private readonly PlayerGeneratorModel _playerGeneratorModel;
        private readonly ICommandPublisher _publisher;

        public HudSectionFactory(
            GeneratorRegistry generatorRegistry,
            PlayerGeneratorModel playerGeneratorModel,
            ICommandPublisher publisher)
        {
            _generatorRegistry = generatorRegistry;
            _playerGeneratorModel = playerGeneratorModel;
            _publisher = publisher;
        }

        public IGameplaySectionView Create(GameplaySectionDefinition definition)
        {
            switch (definition.Id)
            {
                case "generators":
                    return new GeneratorsSectionView(_generatorRegistry, _playerGeneratorModel, _publisher);
                case "crafting":
                    return new PlaceholderSectionView(definition, "Crafting recipes will appear here.");
                case "orders":
                    return new PlaceholderSectionView(definition, "Customer orders will appear here.");
                case "shop":
                    return new PlaceholderSectionView(definition, "Shop catalog will appear here.");
                case "quests":
                    return new PlaceholderSectionView(definition, "Quest progress will appear here.");
                case "talents":
                    return new PlaceholderSectionView(definition, "Talent upgrades will appear here.");
                default:
                    return new PlaceholderSectionView(definition, definition.TabTitle + " section is not available.");
            }
        }
    }
}