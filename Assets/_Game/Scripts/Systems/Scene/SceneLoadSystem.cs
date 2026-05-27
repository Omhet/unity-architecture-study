namespace App.Systems.Scene
{
    using UnityEngine.SceneManagement;
    using Cysharp.Threading.Tasks;

    public class SceneLoadSystem
    {
        private readonly LoadingScreenView _loadingScreenView;

        public SceneLoadSystem(LoadingScreenView loadingScreenView)
        {
            _loadingScreenView = loadingScreenView;
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask();
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }

        public void ShowLoading()
        {
            if (_loadingScreenView != null)
            {
                _loadingScreenView.Show();
            }
        }

        public void HideLoading()
        {
            if (_loadingScreenView != null)
            {
                _loadingScreenView.Hide();
            }
        }
    }
}
