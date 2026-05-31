namespace App.Craft.Core
{
    using App.Products.Core;
    using App.Recipes.Core;
    using App.Resources.Core;

    public class CraftService
    {
        private readonly RecipeRegistry _recipeRegistry;
        private readonly RecipeState _recipeState;
        private readonly ResourceState _resourceState;
        private readonly ProductState _productState;

        public CraftService(RecipeRegistry recipeRegistry, RecipeState recipeState,
        ResourceState resourceState,
        ProductState productState
        )
        {
            _recipeRegistry = recipeRegistry;
            _recipeState = recipeState;
            _resourceState = resourceState;
            _productState = productState;
        }

        public void Craft(string recipeId)
        {
            // Check if player owns the recipe
            if (!_recipeState.IsPlayerOwned(recipeId))
            {
                return;
            }

            // Get recipe definition
            if (!_recipeRegistry.TryGetById(recipeId, out var recipe) || recipe == null)
            {
                return;
            }

            // Check if player has enough resources 
            foreach (var input in recipe.InputResources)
            {
                var resourceId = input.Key;
                var requiredAmount = input.Value;
                if (!_resourceState.HasEnough(resourceId, requiredAmount))
                {
                    return;
                }
            }

            // Deduct input resources
            foreach (var input in recipe.InputResources)
            {
                var resourceId = input.Key;
                var requiredAmount = input.Value;
                _resourceState.AddAmount(resourceId, -requiredAmount);
            }

            // Add output product
            // TODO: Hardcode quantity to 1 for now, but this can be extended in the future if needed.
            _productState.AddAmount(recipe.OutputProductId, 1);
        }
    }
}