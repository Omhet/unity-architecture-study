namespace App.Flow.Handlers
{
    using System;
    using App.Progression.Core;
    using App.Shop.Core;
    using R3;
    using VContainer.Unity;
    using VitalRouter;

    public partial class LevelUpFlowHandler : IInitializable, IDisposable
    {
        private readonly ProgressionState _progressionState;
        private readonly ShopService _shopService;
        private IDisposable _subscription;

        public LevelUpFlowHandler(ProgressionState progressionState, ShopService shopService)
        {
            _progressionState = progressionState;
            _shopService = shopService;
        }

        public void Initialize()
        {
            // subscribe to level changes and refresh shop availability
            _subscription = _progressionState.Level.Subscribe(level => _shopService.RefreshAvailability(level));
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
