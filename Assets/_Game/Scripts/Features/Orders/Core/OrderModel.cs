namespace App.Orders.Core
{
    using ObservableCollections;

    public class OrderModel
    {
        public ObservableList<string> ActiveOrderIds { get; } = new ObservableList<string>();
    }
}
