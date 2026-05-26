namespace App.Menu.Flow
{
    using App.Infra.SceneManagement;
    using Cysharp.Threading.Tasks;
    using VitalRouter;

    [Routes]
    public partial class PlayGameEventHandler
    {
        private readonly SceneLoader _sceneLoader;

        public PlayGameEventHandler(SceneLoader sceneLoader)
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