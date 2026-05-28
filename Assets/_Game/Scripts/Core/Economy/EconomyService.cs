namespace App.Economy.Core
{
    public class EconomyService
    {
        private readonly EconomyState _state;

        public EconomyService(EconomyState state)
        {
            _state = state;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            _state.Balance.Value += amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return false;

            if (_state.Balance.Value >= amount)
            {
                _state.Balance.Value -= amount;
                return true;
            }
            return false;
        }
    }
}
