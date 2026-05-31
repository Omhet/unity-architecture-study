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
        void On(CompleteOrderEvent command)
        {
            _orderService.TryCompleteOrder(command.OrderId);
        }
    }
}