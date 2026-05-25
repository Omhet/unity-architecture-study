using App.Economy.Core;
using App.Inventory.Core;
using System.Linq;

namespace App.Shop.Core
{
    public class ShopService
    {
        private readonly ShopModel _shopModel;
        private readonly EconomyService _economyService;
        private readonly InventoryService _inventoryService;

        public ShopService(
            ShopModel shopModel,
            EconomyService economyService,
            InventoryService inventoryService)
        {
            _shopModel = shopModel;
            _economyService = economyService;
            _inventoryService = inventoryService;
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

        public bool TryBuyItem(string itemId)
        {
            // 1. Find Item
            ShopItem itemToBuy = _shopModel.Items.FirstOrDefault(x => x.ItemId == itemId);
            if (itemToBuy == null) return false;

            // 2. Validate and Spend Currency
            if (_economyService.TrySpend(itemToBuy.Cost))
            {
                // 3. Give Item
                _inventoryService.AddItem(itemToBuy.ItemId, 1);
                return true;
            }

            return false;
        }
    }
}
