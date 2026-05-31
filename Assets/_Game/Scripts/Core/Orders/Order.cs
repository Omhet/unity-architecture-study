namespace App.Orders.Core
{
    public class Order
    {
        public string Id { get; }
        public OrderRequirements Requirements { get; }
        public int Reward { get; }

        public Order(string id, OrderRequirements requirements, int reward)
        {
            Id = id;
            Requirements = requirements;
            Reward = reward;
        }
    }

    public class OrderRequirements
    {
        public string ProductId { get; }
        public int Quantity { get; }

        public OrderRequirements(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}