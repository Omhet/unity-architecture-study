namespace App.Generators.Core
{
    using System.Collections.Generic;
    using App.GameConfig.Core;

    public class ProductRegistry
    {
        private readonly List<ProductDefinition> _products = new List<ProductDefinition>();

        public void Load(ProductCatalogConfig config)
        {
            _products.Clear();

            if (config?.Products == null)
            {
                return;
            }

            for (int i = 0; i < config.Products.Length; i++)
            {
                var product = config.Products[i];
                if (product == null || string.IsNullOrWhiteSpace(product.Id))
                {
                    continue;
                }

                _products.Add(product);
            }
        }

        public bool TryGetById(string productId, out ProductDefinition product)
        {
            product = null;
            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            for (int i = 0; i < _products.Count; i++)
            {
                var current = _products[i];
                if (current != null && current.Id == productId)
                {
                    product = current;
                    return true;
                }
            }

            return false;
        }

        public int Count => _products.Count;

        public ProductDefinition this[int index] => _products[index];
    }
}