namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Recipes.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class RecipeSaveData
    {
        public List<string> OwnedRecipeIds { get; set; } = new List<string>();
    }

    public class RecipeSaveModule : ISaveModule
    {
        private readonly RecipeState _state;

        public string Key => "recipes";

        public RecipeSaveModule(RecipeState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new RecipeSaveData
            {
                OwnedRecipeIds = new List<string>(_state.PlayerOwnedRecipeIds)
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<RecipeSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<RecipeSaveData>(Key);
            _state.PlayerOwnedRecipeIds.Clear();
            foreach (var recipeId in data.OwnedRecipeIds)
            {
                _state.PlayerOwnedRecipeIds.Add(recipeId);
            }
        }
    }
}
