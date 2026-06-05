namespace App.Quests.Core.Conditions
{
    using App.Products.Core;
    using ObservableCollections;
    using R3;

    public class ProductThresholdEvaluator : IConditionEvaluator
    {
        private readonly ProductState _productState;
        private readonly string _targetId;
        private readonly int _threshold;

        public ProductThresholdEvaluator(ProductState productState, string targetId, int threshold)
        {
            _productState = productState;
            _targetId = targetId;
            _threshold = threshold;
        }

        public bool IsMet()
        {
            return _productState.GetAmount(_targetId) >= _threshold;
        }

        public Observable<bool> Observe()
        {
            return _productState.PlayerOwnedProductAmounts.ObserveAdd()
                .Where(e => e.Value.Key == _targetId && e.Value.Value >= _threshold)
                .Select(_ => true)
                .Take(1);
        }
    }
}
