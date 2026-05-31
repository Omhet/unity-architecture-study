namespace App.Orders.Core
{
    using System;
    using System.Linq;
    using App.Economy.Core;
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

        public void CreateNewOrder()
        {
            // Check what products the player has and create an order based on that
            var allProductAmounts = _productState.GetProductAmounts();
            if (!allProductAmounts.Any())
            {
                // If player has no products, create a default order for a basic product
                var basicOrder = new Order(
                id: Guid.NewGuid().ToString(),
                requirements: new OrderRequirements(productId: "wooden_hammer", quantity: 1),
                reward: 1);

                _orderState.ActiveOrders.Add(basicOrder);

                return;
            }

            var randomProduct = allProductAmounts.OrderBy(_ => Guid.NewGuid()).First();
            var randomRequirementQuantity = Math.Min(randomProduct.Value, 5); // Require up to 5 of the product

            var newOrder = new Order
            (
                id: Guid.NewGuid().ToString(),
                requirements: new OrderRequirements(
                    productId: randomProduct.Key,
                    quantity: randomRequirementQuantity
                ),
                reward: 1 * randomRequirementQuantity // TODO: Hardcode reward multiplier for now
            );

            _orderState.ActiveOrders.Add(newOrder);
        }
    }
}