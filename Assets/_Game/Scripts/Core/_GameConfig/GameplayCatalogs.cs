namespace App.GameConfig.Core
{
    using System;

    [Serializable]
    public class GameConfigManifest
    {
        public int SchemaVersion;
        public ConfigCatalogEntry[] Catalogs;
    }

    [Serializable]
    public class ConfigCatalogEntry
    {
        public string Key;
        public string Address;
        public int SchemaVersion;
    }

    [Serializable]
    public class ResourceDefinition
    {
        public string Id;
    }

    [Serializable]
    public class ResourceCatalogConfig
    {
        public ResourceDefinition[] Resources;
    }

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

    [Serializable]
    public class ProductDefinition
    {
        public string Id;
    }

    [Serializable]
    public class ProductCatalogConfig
    {
        public ProductDefinition[] Products;
    }

    public class GameCatalogBundle
    {
        public GameConfigManifest Manifest;
        public ResourceCatalogConfig Resources;
        public GeneratorCatalogConfig Generators;
        public ProductCatalogConfig Products;
    }
}
