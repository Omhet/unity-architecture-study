namespace Flow.Menu
{
    using System;
    using View.Menu;
    using Infra.SceneManagement;
    using R3;
    using VContainer.Unity;
    using Cysharp.Threading.Tasks;

    public class MenuController : IInitializable, IDisposable
    {
        private readonly MainMenuView _view;
        private readonly SceneLoader _sceneLoader;
        private IDisposable _subscription;

        public MenuController(MainMenuView view, SceneLoader sceneLoader)
        {
            _view = view;
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            _subscription = _view.OnPlayClicked.Subscribe(_ =>
            {
                _sceneLoader.LoadSceneAsync("Game").Forget();
            });
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}