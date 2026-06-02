namespace App.Resources.Core
{
    using System;

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
}
