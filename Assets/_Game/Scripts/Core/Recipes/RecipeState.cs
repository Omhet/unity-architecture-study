namespace App.Recipes.Core
{
    using ObservableCollections;

    public class RecipeState
    {
        public ObservableList<string> PlayerOwnedRecipeIds { get; } = new ObservableList<string>();

        public bool IsPlayerOwned(string recipeId)
        {
            if (string.IsNullOrWhiteSpace(recipeId))
            {
                return false;
            }

            for (int i = 0; i < PlayerOwnedRecipeIds.Count; i++)
            {
                if (PlayerOwnedRecipeIds[i] == recipeId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}