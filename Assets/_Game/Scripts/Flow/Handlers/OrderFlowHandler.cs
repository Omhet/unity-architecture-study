namespace App.Flow.Handlers
{
    using System;
    using System.Threading;
    using App.Flow.Events;
    using App.Orders.Core;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class OrderFlowHandler
    {
        private readonly OrderState _orderState;
        private readonly OrderService _orderService;
        private CancellationTokenSource _orderGenerationCts;

        public OrderFlowHandler(OrderState orderState, OrderService orderService)
        {
            _orderState = orderState;
            _orderService = orderService;
        }

        [Route]
        void On(StartGameEvent _)
        {
            // TODO: This is just for testing, remove this when we have a proper order generation flow
            _orderService.CreateNewOrder();
            // Start timer and generate new orders at intervals
            _orderGenerationCts?.Cancel();
            _orderGenerationCts?.Dispose();
            _orderGenerationCts = new CancellationTokenSource();
            var token = _orderGenerationCts.Token;

            UniTask.Void(async () =>
            {
                try
                {
                    var interval = TimeSpan.FromSeconds(5); // adjust interval as needed
                    while (!token.IsCancellationRequested)
                    {
                        await UniTask.Delay(interval, cancellationToken: token);
                        if (token.IsCancellationRequested) break;
                        if (_orderState.ActiveOrders.Count >= 6) continue; // limit max active orders
                        _orderService.CreateNewOrder();
                    }
                }
                catch (OperationCanceledException)
                {
                    // expected on cancellation
                }
            });
        }

        [Route]
        void On(CompleteOrderEvent command)
        {
            _orderService.TryCompleteOrder(command.OrderId);
        }
    }
}