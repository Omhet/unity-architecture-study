namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Shop.Core;
    using VitalRouter;

    [Routes]
    public partial class ShopFlowHandler
    {
        private readonly ShopService _shopService;

        public ShopFlowHandler(ShopService shopService)
        {
            _shopService = shopService;
        }

        [Route]
        void On(BuyShopItemEvent command)
        {
            _shopService.TryToBuy(command.ShopItemId);
        }
    }
}
