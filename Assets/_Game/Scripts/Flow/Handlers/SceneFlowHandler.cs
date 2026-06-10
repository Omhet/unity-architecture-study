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

            // Initialize game config before loading gameplay scene
            // TODO: Rename the service to be more scpecific that it loads config data
            await _initializationService.InitializeAsync();

            // Load save data after config hydration completes
            int activeSlot = _slotManager.GetActiveSlot();
            await _saveLoadSystem.LoadSlotAsync(activeSlot);

            // Load gameplay scene after config and save data are ready, 
            // so that scene can access them during its initialization
            await _sceneLoader.LoadSceneAsync("Game");

            _sceneLoader.HideLoading();

            await _publisher.PublishAsync(new StartGameEvent());
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(ExitToMenuEvent _)
        {
            _sceneLoader.ShowLoading();

            // Save before exiting gameplay scene
            int activeSlot = _slotManager.GetActiveSlot();
            await _saveLoadSystem.SaveSlotAsync(activeSlot);

            await _sceneLoader.LoadSceneAsync("Menu");

            _sceneLoader.HideLoading();
        }
    }
}
