namespace App.Products.Core
{
    using ObservableCollections;

    public class ProductState
    {
        public ObservableList<string> PlayerOwnedProductIds { get; } = new ObservableList<string>();

        public bool IsPlayerOwned(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            for (int i = 0; i < PlayerOwnedProductIds.Count; i++)
            {
                if (PlayerOwnedProductIds[i] == productId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}