namespace App.Resources.Core
{
    using System.Collections.Generic;
    using ObservableCollections;

    public class ResourceModel
    {
        public ObservableDictionary<string, int> Balances { get; } = new ObservableDictionary<string, int>();

        public int GetAmount(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return 0;
            }

            return Balances.TryGetValue(resourceId, out int amount) ? amount : 0;
        }

        public bool HasEnough(string resourceId, int amount)
        {
            return amount > 0 && GetAmount(resourceId) >= amount;
        }

        public IEnumerable<KeyValuePair<string, int>> EnumerateBalances()
        {
            return Balances;
        }

        public void Clear()
        {
            Balances.Clear();
        }

        public void SetAmount(string resourceId, int amount)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return;
            }

            Balances[resourceId] = amount;
        }

        public void AddAmount(string resourceId, int delta)
        {
            if (string.IsNullOrWhiteSpace(resourceId) || delta == 0)
            {
                return;
            }

            Balances[resourceId] = GetAmount(resourceId) + delta;
        }
    }
}
