namespace App.Craft.Core
{
    using System;
    using App.Products.Core;
    using App.Recipes.Core;
    using App.Resources.Core;
    using App.Talents.Core;

    public class CraftService
    {
        private readonly RecipeRegistry _recipeRegistry;
        private readonly RecipeState _recipeState;
        private readonly ResourceState _resourceState;
        private readonly ProductState _productState;
        private readonly TalentService _talentService;

        public CraftService(RecipeRegistry recipeRegistry, RecipeState recipeState,
        ResourceState resourceState,
        ProductState productState,
        TalentService talentService
        )
        {
            _recipeRegistry = recipeRegistry;
            _recipeState = recipeState;
            _resourceState = resourceState;
            _productState = productState;
            _talentService = talentService;
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
            int baseAmount = 1;
            float multiplier = _talentService.GetMultiplier("craft_boost");
            int amount = (int)Math.Ceiling(baseAmount * multiplier);
            _productState.AddAmount(recipe.OutputProductId, amount);
        }
    }
}