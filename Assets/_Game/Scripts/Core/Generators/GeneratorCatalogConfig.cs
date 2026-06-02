namespace App.Generators.Core
{
    using System;

    [Serializable]
    public class GeneratorDefinition
    {
        public string Id;
        public string ResourceId;
    }

    [Serializable]
    public class GeneratorCatalogConfig
    {
        public GeneratorDefinition[] Generators;
    }
}
