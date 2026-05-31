namespace App.Generators.Core
{
    using System.Linq;
    using App.Economy.Core;
    using App.Orders.Core;
    using App.Products.Core;

    public class OrderService
    {
        private readonly OrderState _orderState;
        private readonly ProductState _productState;
        private readonly EconomyService _economyService;

        public OrderService(
            OrderState orderState,
            ProductState productState,
            EconomyService economyService)
        {
            _orderState = orderState;
            _productState = productState;
            _economyService = economyService;
        }

        public bool TryCompleteOrder(string orderId)
        {
            // Check if the order is active
            if (!_orderState.IsOrderActive(orderId))
            {
                return false;
            }

            // Get the active order
            var order = _orderState.ActiveOrders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return false;
            }

            // Check if player has enough products to fulfill the order
            var requirements = order.Requirements;
            var playerOwnedProductAmount = _productState.GetAmount(requirements.ProductId);
            if (playerOwnedProductAmount < requirements.Quantity)
            {
                return false;
            }

            // Deduct the required quantity from the product
            _productState.AddAmount(requirements.ProductId, -requirements.Quantity);

            // Add the reward to the economy
            _economyService.Add(order.Reward);

            // Remove the completed order from active orders
            _orderState.ActiveOrders.Remove(order);

            return true;
        }
    }
}