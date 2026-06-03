namespace App.Hud.View
{
    using ObservableCollections;
    using App.Flow.Events;
    using App.Shop.Core;
    using App.View;
    using R3;
    using System;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class ShopSectionView : GameplaySectionViewBase
    {
        private readonly ShopState _shopState;
        private readonly ShopRegistry _shopRegistry;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _subscription;

        public ShopSectionView(
            ICommandPublisher publisher,
            ShopState shopState,
            ShopRegistry shopRegistry
            )
            : base(new GameplaySectionDefinition("shop", "Shop", 0))
        {
            _publisher = publisher;
            _shopState = shopState;
            _shopRegistry = shopRegistry;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("shop-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            _list = new VisualElement();
            _list.AddToClassList("shop-list");

            root.Add(_list);
        }

        protected override void Bind()
        {
            _subscription?.Dispose();

            if (_shopState == null)
            {
                return;
            }

            var updates = Observable.Merge(
                _shopState.AvailableShopItemIds.ObserveAdd().Select(_ => Unit.Default),
                _shopState.AvailableShopItemIds.ObserveRemove().Select(_ => Unit.Default),
                _shopState.AvailableShopItemIds.ObserveReplace().Select(_ => Unit.Default),
                _shopState.AvailableShopItemIds.ObserveReset().Select(_ => Unit.Default));

            _subscription = Observable.Return(Unit.Default)
                .Concat(updates)
                .Subscribe(_ => RebuildRows());
        }

        protected override void Unbind()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void RebuildRows()
        {
            if (_list == null || _shopState == null)
            {
                return;
            }

            _list.Clear();

            for (int i = 0; i < _shopState.AvailableShopItemIds.Count; i++)
            {
                var shopItemId = _shopState.AvailableShopItemIds[i];
                _list.Add(BuildShopItemRow(shopItemId));
            }
        }

        private VisualElement BuildShopItemRow(string shopItemId)
        {
            var row = new VisualElement();
            row.AddToClassList("shop-item-card");

            string itemName = shopItemId;
            int price = 0;

            if (_shopRegistry.TryGetAny(shopItemId, out _, out var definition))
            {
                itemName = definition.ItemId;
                price = definition.Price;
            }

            var nameLabel = new Label(itemName);
            nameLabel.AddToClassList("shop-item-name");
            row.Add(nameLabel);

            var priceLabel = new Label(price.ToString());
            priceLabel.AddToClassList("shop-item-price");
            row.Add(priceLabel);

            var buyButton = new Button(() => HandleBuyClicked(shopItemId))
            {
                text = "Buy"
            };
            buyButton.AddToClassList("shop-buy-button");

            row.Add(buyButton);

            return row;
        }

        private void HandleBuyClicked(string shopItemId)
        {
            _publisher.PublishAsync(new BuyShopItemEvent(shopItemId));
        }
    }
}
