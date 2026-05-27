namespace App.Shop.Core
{
    public class ShopService
    {
        private readonly ShopModel _shopModel;

        public ShopService(
            ShopModel shopModel)
        {
            _shopModel = shopModel;
        }

        public void LoadCatalog(ShopConfig config)
        {
            _shopModel.Items.Clear();
            if (config?.AvailableItems != null)
            {
                foreach (var item in config.AvailableItems)
                {
                    _shopModel.Items.Add(item);
                }
            }
        }
    }
}
