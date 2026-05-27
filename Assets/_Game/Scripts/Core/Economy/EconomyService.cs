namespace App.Economy.Core
{
    public class EconomyService
    {
        private readonly EconomyModel _model;

        public EconomyService(EconomyModel model)
        {
            _model = model;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            _model.Balance.Value += amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return false;

            if (_model.Balance.Value >= amount)
            {
                _model.Balance.Value -= amount;
                return true;
            }
            return false;
        }
    }
}
