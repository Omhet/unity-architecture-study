namespace App.Flow.Handlers
{
    using System;
    using App.Flow.Events;
    using App.Progression.Core;
    using App.Shop.Core;
    using R3;
    using VitalRouter;

    [Routes]
    public partial class LevelUpFlowHandler : IDisposable
    {
        private readonly ProgressionState _progressionState;
        private readonly ShopService _shopService;
        private IDisposable _subscription;

        public LevelUpFlowHandler(ProgressionState progressionState, ShopService shopService)
        {
            _progressionState = progressionState;
            _shopService = shopService;
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

            _shopService.RefreshAvailability(pair.currentLevel);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
