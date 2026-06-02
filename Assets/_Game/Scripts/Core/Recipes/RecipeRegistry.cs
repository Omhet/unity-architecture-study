namespace App.Recipes.Core
{
    using System.Collections.Generic;

    public class RecipeRegistry
    {
        private readonly List<RecipeDefinition> _recipes = new List<RecipeDefinition>();

        public void Load(RecipeCatalogConfig config)
        {
            _recipes.Clear();

            if (config?.Recipes == null)
            {
                return;
            }

            for (int i = 0; i < config.Recipes.Length; i++)
            {
                var recipe = config.Recipes[i];
                if (recipe == null || string.IsNullOrWhiteSpace(recipe.Id))
                {
                    continue;
                }

                _recipes.Add(recipe);
            }
        }

        public bool TryGetById(string recipeId, out RecipeDefinition recipe)
        {
            recipe = null;
            if (string.IsNullOrWhiteSpace(recipeId))
            {
                return false;
            }

            for (int i = 0; i < _recipes.Count; i++)
            {
                var current = _recipes[i];
                if (current != null && current.Id == recipeId)
                {
                    recipe = current;
                    return true;
                }
            }

            return false;
        }

        public int Count => _recipes.Count;

        public RecipeDefinition this[int index] => _recipes[index];
    }
}