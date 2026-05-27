namespace App.Products.Core
{
    using ObservableCollections;

    public class ProductInventoryModel
    {
        public ObservableDictionary<string, int> Products { get; } = new ObservableDictionary<string, int>();

        public int GetAmount(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return 0;
            }

            return Products.TryGetValue(productId, out int amount) ? amount : 0;
        }

        public bool HasEnough(string productId, int amount)
        {
            return amount > 0 && GetAmount(productId) >= amount;
        }
    }
}
