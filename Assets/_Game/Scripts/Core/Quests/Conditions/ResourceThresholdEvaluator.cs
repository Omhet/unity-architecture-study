namespace App.Quests.Core.Conditions
{
    using App.Resources.Core;
    using ObservableCollections;
    using R3;

    public class ResourceThresholdEvaluator : IConditionEvaluator
    {
        private readonly ResourceState _resourceState;
        private readonly string _targetId;
        private readonly int _threshold;

        public ResourceThresholdEvaluator(ResourceState resourceState, string targetId, int threshold)
        {
            _resourceState = resourceState;
            _targetId = targetId;
            _threshold = threshold;
        }

        public bool IsMet()
        {
            return _resourceState.GetAmount(_targetId) >= _threshold;
        }

        public Observable<bool> Observe()
        {
            return _resourceState.Balances.ObserveAdd()
                .Where(e => e.Value.Key == _targetId && e.Value.Value >= _threshold)
                .Select(_ => true)
                .Take(1);
        }
    }
}
