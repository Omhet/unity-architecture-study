namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Systems.Scene;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class PlayGameEventHandler
    {
        private readonly SceneLoadSystem _sceneLoader;

        public PlayGameEventHandler(SceneLoadSystem sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        [Route(CommandOrdering.Drop)]
        async UniTask On(PlayGameEvent _)
        {
            await _sceneLoader.LoadSceneAsync("Game");
        }
    }
}