namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Boot.Utility;
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

            ConfigValidationHelper.ValidateUniqueIds(config.Products, x => x?.Id, "product", errors);
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<ProductCatalogConfig>(Key);
            _productRegistry.Load(config);
        }
    }
}
