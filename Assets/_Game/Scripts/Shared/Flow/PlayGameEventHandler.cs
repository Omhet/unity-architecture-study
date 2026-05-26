namespace App.Shared.Flow
{
    using App.Systems.Scene;
    using Cysharp.Threading.Tasks;
    using VitalRouter;
    using App.Menu.Core;

    [Routes]
    public partial class PlayGameEventHandler
    {
        private readonly SceneLoadSystem _sceneLoader;

        public PlayGameEventHandler(SceneLoadSystem sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        [Route]
        async UniTask On(PlayGameEvent _)
        {
            await _sceneLoader.LoadSceneAsync("Game");
        }
    }
}