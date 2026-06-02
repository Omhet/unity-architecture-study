namespace App.Recipes.Core
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RecipeDefinition
    {
        public string Id;
        public Dictionary<string, int> InputResources;
        public string OutputProductId;
    }

    [Serializable]
    public class RecipeCatalogConfig
    {
        public RecipeDefinition[] Recipes;
    }
}
