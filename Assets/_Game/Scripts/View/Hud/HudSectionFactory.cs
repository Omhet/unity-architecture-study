namespace App.Hud.View
{
    using App.Generators.Core;
    using App.Products.Core;
    using App.Recipes.Core;
    using App.View;
    using VitalRouter;

    public class HudSectionFactory
    {
        private readonly GeneratorState _generatorState;
        private readonly RecipeState _recipeState;
        private readonly RecipeRegistry _recipeRegistry;
        private readonly ProductState _productState;
        private readonly ICommandPublisher _publisher;

        public HudSectionFactory(
            GeneratorState generatorState,
            RecipeState recipeState,
            RecipeRegistry recipeRegistry,
            ProductState productState,
            ICommandPublisher publisher)
        {
            _generatorState = generatorState;
            _recipeState = recipeState;
            _recipeRegistry = recipeRegistry;
            _productState = productState;
            _publisher = publisher;
        }

        public IGameplaySectionView Create(GameplaySectionDefinition definition)
        {
            switch (definition.Id)
            {
                case "generators":
                    return new GeneratorsSectionView(_publisher, _generatorState);
                case "crafting":
                    return new CraftSectionView(_publisher, _recipeState, _recipeRegistry, _productState);
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