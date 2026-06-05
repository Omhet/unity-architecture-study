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
        private IDisposable _subscription;

        public LevelUpFlowHandler(ProgressionState progressionState, ShopService shopService, TalentService talentService)
        {
            _progressionState = progressionState;
            _shopService = shopService;
            _talentService = talentService;
        }

        [Route]
        void On(StartGameEvent _)
        {
            _subscription = _progressionState.Level.Pairwise().Subscribe(HandleLevelChange);
        }

        private void HandleLevelChange((int previousLevel, int currentLevel) pair)
        {
            if (pair.previousLevel == pair.currentLevel)
            {
                return;
            }

            int levelsGained = pair.currentLevel - pair.previousLevel;
            for (int i = 0; i < levelsGained; i++)
            {
                _talentService.AddPoint();
            }

            _shopService.RefreshAvailability(pair.currentLevel);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
