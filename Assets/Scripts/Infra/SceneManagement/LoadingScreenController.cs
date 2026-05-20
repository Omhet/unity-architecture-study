namespace Infra.SceneManagement
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Cysharp.Threading.Tasks;

    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingOverlay;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_loadingOverlay != null)
            {
                _loadingOverlay.SetActive(false);
            }
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (_loadingOverlay != null) _loadingOverlay.SetActive(true);

            // Optional future step: Trigger PrimeTween fade-in here

            await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask();

            // Optional future step: Trigger PrimeTween fade-out here

            if (_loadingOverlay != null) _loadingOverlay.SetActive(false);
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}
