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
        private readonly ICommandPublisher _publisher;

        public SceneFlowHandler(SceneLoadSystem sceneLoader, GameConfigInitializationSystem initializationService, ICommandPublisher publisher)
        {
            _sceneLoader = sceneLoader;
            _initializationService = initializationService;
            _publisher = publisher;
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(PlayGameEvent _)
        {
            _sceneLoader.ShowLoading();

            await _sceneLoader.LoadSceneAsync("Game");
            await _initializationService.InitializeAsync();

            _sceneLoader.HideLoading();

            await _publisher.PublishAsync(new StartGameEvent());
        }
    }
}
