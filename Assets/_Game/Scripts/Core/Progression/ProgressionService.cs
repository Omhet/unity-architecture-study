namespace App.Progression.Core
{
    public class ProgressionService
    {
        private readonly ProgressionState _progressionState;
        private readonly ProgressionRegistry _progressionRegistry;

        public ProgressionService(ProgressionState progressionState, ProgressionRegistry progressionRegistry)
        {
            _progressionState = progressionState;
            _progressionRegistry = progressionRegistry;
        }

        public void AddXp(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            var xp = _progressionState.Xp.Value + amount;

            while (true)
            {
                var threshold = _progressionRegistry.GetNextLevelXpForLevel(_progressionState.Level.Value);

                if (threshold <= 0)
                {
                    // No further levels defined - just store XP
                    _progressionState.Xp.Value = xp;
                    break;
                }

                if (xp >= threshold)
                {
                    xp -= threshold;
                    _progressionState.Level.Value = _progressionState.Level.Value + 1;
                    // continue loop to handle multiple level-ups in one addition
                    continue;
                }

                _progressionState.Xp.Value = xp;
                break;
            }
        }
    }
}
