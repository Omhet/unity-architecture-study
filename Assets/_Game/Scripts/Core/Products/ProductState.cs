namespace App.Products.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using ObservableCollections;

    public class ProductState
    {
        public ObservableDictionary<string, int> PlayerOwnedProductAmounts { get; } = new ObservableDictionary<string, int>();

        public int GetAmount(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return 0;
            }

            return PlayerOwnedProductAmounts.TryGetValue(resourceId, out int amount) ? amount : 0;
        }

        public bool HasEnough(string resourceId, int amount)
        {
            return amount > 0 && GetAmount(resourceId) >= amount;
        }

        public IEnumerable<KeyValuePair<string, int>> GetProductAmounts()
        {
            return PlayerOwnedProductAmounts;
        }

        public void Clear()
        {
            PlayerOwnedProductAmounts.Clear();
        }

        public void SetAmount(string resourceId, int amount)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return;
            }

            PlayerOwnedProductAmounts[resourceId] = amount;
        }

        public void AddAmount(string resourceId, int delta)
        {
            if (string.IsNullOrWhiteSpace(resourceId) || delta == 0)
            {
                return;
            }

            PlayerOwnedProductAmounts[resourceId] = GetAmount(resourceId) + delta;
        }
    }
}