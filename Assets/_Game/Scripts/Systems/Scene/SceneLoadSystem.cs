namespace App.Systems.Scene
{
    using UnityEngine.SceneManagement;
    using Cysharp.Threading.Tasks;

    public class SceneLoadSystem
    {
        private LoadingScreenView _loadingScreenView;

        public SceneLoadSystem(LoadingScreenView loadingScreenView)
        {
            _loadingScreenView = loadingScreenView;
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (_loadingScreenView != null)
            {
                _loadingScreenView.Show();
            }

            await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask();

            if (_loadingScreenView != null)
            {
                _loadingScreenView.Hide();
            }
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}
