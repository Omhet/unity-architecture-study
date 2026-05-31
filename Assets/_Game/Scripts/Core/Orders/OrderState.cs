namespace App.Orders.Core
{
    using ObservableCollections;

    public class OrderState
    {
        public ObservableList<Order> ActiveOrders { get; } = new ObservableList<Order>();

        public bool IsOrderActive(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return false;
            }

            for (int i = 0; i < ActiveOrders.Count; i++)
            {
                if (ActiveOrders[i].Id == orderId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}