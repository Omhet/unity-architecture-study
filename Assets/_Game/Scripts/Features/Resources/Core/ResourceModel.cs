namespace App.Resources.Core
{
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
    }
}
