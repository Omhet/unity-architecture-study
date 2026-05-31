namespace App.Hud.View
{
    using ObservableCollections;
    using App.Flow.Events;
    using App.View;
    using R3;
    using System;
    using UnityEngine.UIElements;
    using VitalRouter;
    using App.Products.Core;
    using App.Orders.Core;
    using System.Linq;

    public class OrdersSectionView : GameplaySectionViewBase
    {
        private readonly OrderState _orderState;
        private readonly ProductState _productState;
        private readonly ICommandPublisher _publisher;
        private VisualElement _ordersList;
        private VisualElement _productsList;
        private IDisposable _ownedOrdersSubscription;
        private IDisposable _ownedProductsSubscription;

        public OrdersSectionView(
            ICommandPublisher publisher,
            OrderState orderState,
            ProductState productState
            )
            : base(new GameplaySectionDefinition("orders", "Orders", 0))
        {
            _orderState = orderState;
            _productState = productState;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("orders-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            var content = new VisualElement();
            content.AddToClassList("orders-section-content");

            _ordersList = new VisualElement();
            _ordersList.AddToClassList("orders-list");

            _productsList = new VisualElement();
            _productsList.AddToClassList("products-list");

            content.Add(_ordersList);
            content.Add(_productsList);

            root.Add(content);
        }

        protected override void Bind()
        {
            _ownedOrdersSubscription?.Dispose();
            _ownedProductsSubscription?.Dispose();

            if (_orderState == null || _productState == null)
            {
                return;
            }

            var ordersUpdates = Observable.Merge(
                _orderState.ActiveOrders.ObserveAdd().Select(_ => Unit.Default),
                _orderState.ActiveOrders.ObserveRemove().Select(_ => Unit.Default),
                _orderState.ActiveOrders.ObserveReplace().Select(_ => Unit.Default),
                _orderState.ActiveOrders.ObserveReset().Select(_ => Unit.Default));

            _ownedOrdersSubscription = Observable.Return(Unit.Default)
                .Concat(ordersUpdates)
                .Subscribe(_ => RebuildOrderRows());

            var productsUpdates = Observable.Merge(
                _productState.PlayerOwnedProductAmounts.ObserveAdd().Select(_ => Unit.Default),
                _productState.PlayerOwnedProductAmounts.ObserveRemove().Select(_ => Unit.Default),
                _productState.PlayerOwnedProductAmounts.ObserveReplace().Select(_ => Unit.Default),
                _productState.PlayerOwnedProductAmounts.ObserveReset().Select(_ => Unit.Default));

            _ownedProductsSubscription = Observable.Return(Unit.Default)
                .Concat(productsUpdates)
                .Subscribe(_ => RebuildProductRows());
        }

        protected override void Unbind()
        {
            _ownedOrdersSubscription?.Dispose();
            _ownedOrdersSubscription = null;

            _ownedProductsSubscription?.Dispose();
            _ownedProductsSubscription = null;
        }

        private VisualElement BuildOrderRow(string orderId)
        {
            var row = new VisualElement();
            row.AddToClassList("order-card");

            var title = new Label(orderId);
            title.AddToClassList("order-title");
            row.Add(title);

            // Show requirements list
            var order = _orderState.ActiveOrders.FirstOrDefault(o => o.Id == orderId);
            var requirements = order.Requirements;
            var requirementsLabel = new Label($"Requires: {requirements.Quantity} x {requirements.ProductId}");
            requirementsLabel.AddToClassList("order-requirements");
            row.Add(requirementsLabel);

            // Show reward
            var rewardLabel = new Label($"Reward: {order.Reward}");
            rewardLabel.AddToClassList("order-reward");
            row.Add(rewardLabel);

            var completeButton = new Button(() => HandleCompleteClicked(orderId))
            {
                text = "Complete"
            };
            completeButton.AddToClassList("complete-button");

            row.Add(completeButton);

            return row;
        }

        private void HandleCompleteClicked(string orderId)
        {
            _publisher.PublishAsync(new CompleteOrderEvent(orderId));
        }

        private void BuildProductRow(string productId, int productAmount)
        {
            var row = new VisualElement();
            row.AddToClassList("product-list-item");

            var title = new Label($"{productId}: {productAmount}");
            title.AddToClassList("product-title");
            row.Add(title);

            _productsList.Add(row);
        }

        private void RebuildOrderRows()
        {
            if (_ordersList == null || _orderState == null)
            {
                return;
            }

            _ordersList.Clear();

            foreach (var order in _orderState.ActiveOrders)
            {
                var row = BuildOrderRow(order.Id);
                _ordersList.Add(row);
            }
        }

        private void RebuildProductRows()
        {
            if (_productsList == null || _productState == null)
            {
                return;
            }

            _productsList.Clear();

            foreach (var pair in _productState.EnumerateAmounts())
            {
                var productId = pair.Key;
                var productAmount = pair.Value;

                BuildProductRow(productId, productAmount);
            }
        }
    }
}
