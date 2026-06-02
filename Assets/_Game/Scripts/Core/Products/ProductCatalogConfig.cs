namespace App.Products.Core
{
    using System;

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
}
