using VitalRouter;

namespace App.Flow.Events
{
    public readonly struct PlayGameEvent : ICommand
    {
    }

    public readonly struct GenerateFromGeneratorEvent : ICommand
    {
        public readonly string GeneratorId;

        public GenerateFromGeneratorEvent(string generatorId)
        {
            GeneratorId = generatorId;
        }
    }

    public readonly struct CraftRecipeEvent : ICommand
    {
        public readonly string RecipeId;

        public CraftRecipeEvent(string recipeId)
        {
            RecipeId = recipeId;
        }
    }
}
