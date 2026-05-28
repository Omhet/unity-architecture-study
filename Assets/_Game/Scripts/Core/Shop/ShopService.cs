namespace App.Shop.Core
{
    public class ShopService
    {
        private readonly ShopRegistry _shopRegistry;
        private readonly ShopModel _shopModel;

        public ShopService(
            ShopRegistry shopRegistry,
            ShopModel shopModel)
        {
            _shopRegistry = shopRegistry;
            _shopModel = shopModel;
        }

        public void LoadCatalog(ShopConfig config)
        {
            _shopRegistry.Load(config);
            _shopModel.Items.Clear();
            if (config?.AvailableItems != null)
            {
                foreach (var item in config.AvailableItems)
                {
                    if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
                    {
                        continue;
                    }

                    _shopModel.Items.Add(new ShopItemState
                    {
                        ItemId = item.ItemId,
                        IsVisible = item.InitialVisibility,
                        IsBuyable = item.InitialBuyability,
                        IsPurchased = false
                    });
                }
            }
        }
    }
}
