namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Systems.Configuration;
    using App.Systems.Scene;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class SceneFlowHandler
    {
        private readonly SceneLoadSystem _sceneLoader;
        private readonly GameConfigInitializationSystem _initializationService;

        public SceneFlowHandler(SceneLoadSystem sceneLoader, GameConfigInitializationSystem initializationService)
        {
            _sceneLoader = sceneLoader;
            _initializationService = initializationService;
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(PlayGameEvent _)
        {
            _sceneLoader.ShowLoading();

            await _sceneLoader.LoadSceneAsync("Game");
            await _initializationService.InitializeAsync();

            _sceneLoader.HideLoading();
        }
    }
}
