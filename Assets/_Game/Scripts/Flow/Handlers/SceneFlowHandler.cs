namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Systems.Configuration;
    using App.Systems.Scene;
    using App.Systems.Saving.Orchestration;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class SceneFlowHandler
    {
        private readonly SceneLoadSystem _sceneLoader;
        private readonly GameConfigInitializationSystem _initializationService;
        private readonly SaveLoadSystem _saveLoadSystem;
        private readonly SlotManager _slotManager;
        private readonly ICommandPublisher _publisher;

        public SceneFlowHandler(
            SceneLoadSystem sceneLoader,
            GameConfigInitializationSystem initializationService,
            SaveLoadSystem saveLoadSystem,
            SlotManager slotManager,
            ICommandPublisher publisher)
        {
            _sceneLoader = sceneLoader;
            _initializationService = initializationService;
            _saveLoadSystem = saveLoadSystem;
            _slotManager = slotManager;
            _publisher = publisher;
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(PlayGameEvent _)
        {
            _sceneLoader.ShowLoading();

            await _sceneLoader.LoadSceneAsync("Game");
            await _initializationService.InitializeAsync();

            // Load save data after config hydration completes
            int activeSlot = _slotManager.GetActiveSlot();
            await _saveLoadSystem.LoadSlotAsync(activeSlot);

            _sceneLoader.HideLoading();

            await _publisher.PublishAsync(new StartGameEvent());
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(ExitToMenuEvent _)
        {
            _sceneLoader.ShowLoading();

            await _sceneLoader.LoadSceneAsync("Menu");

            // Save before exiting gameplay scene
            int activeSlot = _slotManager.GetActiveSlot();
            await _saveLoadSystem.SaveSlotAsync(activeSlot);

            _sceneLoader.HideLoading();
        }
    }
}
