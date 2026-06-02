namespace App.Boot.ConfigModules
{
    using System;
    using System.Collections.Generic;
    using App.Products.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class ProductConfigModule : IConfigModule
    {
        private readonly ProductRegistry _productRegistry;

        public string Key => "products";

        public ProductConfigModule(ProductRegistry productRegistry)
        {
            _productRegistry = productRegistry;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<ProductCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var config = bundle.GetConfig<ProductCatalogConfig>(Key);
            if (config?.Products == null)
            {
                errors.Add("Missing products catalog.");
                return;
            }

            ValidateUniqueIds(config.Products, x => x?.Id, "product", errors);
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<ProductCatalogConfig>(Key);
            _productRegistry.Load(config);
        }

        private static void ValidateUniqueIds<T>(IEnumerable<T> items, Func<T, string> selector, string itemType, List<string> errors)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                string id = selector(item);
                if (string.IsNullOrWhiteSpace(id))
                {
                    errors.Add("Found " + itemType + " with missing id.");
                    continue;
                }

                if (!ids.Add(id))
                {
                    errors.Add("Duplicate " + itemType + " id: " + id);
                }
            }
        }
    }
}
