namespace App.Talents.Core
{
    using System.Collections.Generic;

    public class TalentService
    {
        private readonly TalentState _state;
        private readonly TalentRegistry _registry;

        public TalentService(TalentState state, TalentRegistry registry)
        {
            _state = state;
            _registry = registry;
        }

        public void AddPoint()
        {
            _state.AvailablePoints.Value++;
        }

        public bool TryPurchase(string talentId)
        {
            if (!_registry.TryGetById(talentId, out var talent))
                return false;

            int currentSpent = _state.PointsSpent.GetValueOrDefault(talentId);
            if (currentSpent >= talent.MaxPoints)
                return false;

            if (_state.AvailablePoints.Value < talent.Cost)
                return false;

            _state.AvailablePoints.Value -= talent.Cost;
            _state.PointsSpent[talentId] = currentSpent + 1;

            return true;
        }

        public float GetMultiplier(string talentId)
        {
            if (!_registry.TryGetById(talentId, out var talent))
                return 1.0f;

            int pointsSpent = _state.PointsSpent.GetValueOrDefault(talentId);
            return 1.0f + (pointsSpent * talent.IncreasePerPoint);
        }

        public int GetPointsSpent(string talentId)
        {
            return _state.PointsSpent.GetValueOrDefault(talentId);
        }
    }
}
