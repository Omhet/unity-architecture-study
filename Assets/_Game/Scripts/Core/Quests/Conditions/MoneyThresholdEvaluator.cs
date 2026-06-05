namespace App.Quests.Core.Conditions
{
    using App.Economy.Core;
    using R3;

    public class MoneyThresholdEvaluator : IConditionEvaluator
    {
        private readonly EconomyState _economyState;
        private readonly int _threshold;

        public MoneyThresholdEvaluator(EconomyState economyState, int threshold)
        {
            _economyState = economyState;
            _threshold = threshold;
        }

        public bool IsMet()
        {
            return _economyState.Balance.Value >= _threshold;
        }

        public Observable<bool> Observe()
        {
            return _economyState.Balance.Where(balance => balance >= _threshold).Select(_ => true).Take(1);
        }
    }
}
