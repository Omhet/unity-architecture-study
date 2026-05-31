namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Orders.Core;
    using VitalRouter;

    [Routes]
    public partial class OrderFlowHandler
    {
        private readonly OrderService _orderService;

        public OrderFlowHandler(OrderService orderService)
        {
            _orderService = orderService;
        }

        [Route]
        void On(StartGameEvent _)
        {
            // TODO: This is just for testing, remove this when we have a proper order generation flow
            _orderService.CreateNewOrder();
        }

        [Route]
        void On(CompleteOrderEvent command)
        {
            bool isCompleted = _orderService.TryCompleteOrder(command.OrderId);

            if (isCompleted)
            {
                _orderService.CreateNewOrder();
            }
        }
    }
}