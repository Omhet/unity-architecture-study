namespace App.Flow.Handlers
{
    using System;
    using App.Flow.Events;
    using App.Progression.Core;
    using App.Shop.Core;
    using App.Talents.Core;
    using R3;
    using VitalRouter;

    [Routes]
    public partial class LevelUpFlowHandler : IDisposable
    {
        private readonly ProgressionState _progressionState;
        private readonly ShopService _shopService;
        private readonly TalentService _talentService;
        private IDisposable _newLevelSubscription;
        private IDisposable _newLevelPairSubscription;

        public LevelUpFlowHandler(ProgressionState progressionState, ShopService shopService, TalentService talentService)
        {
            _progressionState = progressionState;
            _shopService = shopService;
            _talentService = talentService;
        }

        [Route]
        void On(StartGameEvent _)
        {
            // This fires only when new value is different from the previous one
            _newLevelPairSubscription = _progressionState.Level.Pairwise().Subscribe(HandleLevelChange);

            // This fires on every value change and on initial value
            _newLevelSubscription = _progressionState.Level.Subscribe(HandleLevelChange);
        }

        private void HandleLevelChange(int newLevel)
        {
            _shopService.RefreshAvailability(newLevel);
        }

        private void HandleLevelChange((int previousLevel, int currentLevel) pair)
        {
            int levelsGained = pair.currentLevel - pair.previousLevel;
            for (int i = 0; i < levelsGained; i++)
            {
                _talentService.AddPoint();
            }
        }

        public void Dispose()
        {
            _newLevelSubscription?.Dispose();
            _newLevelPairSubscription?.Dispose();
        }
    }
}
