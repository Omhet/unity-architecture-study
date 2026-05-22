namespace Infra.SceneManagement
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;
    using Cysharp.Threading.Tasks;

    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        private VisualElement _rootElement;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_uiDocument != null)
            {
                _rootElement = _uiDocument.rootVisualElement;
                if (_rootElement != null)
                {
                    _rootElement.style.display = DisplayStyle.None;
                }
            }
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (_rootElement != null) _rootElement.style.display = DisplayStyle.Flex;

            // Optional future step: Trigger fade-in here

            await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask();

            // Optional future step: Trigger fade-out here

            if (_rootElement != null) _rootElement.style.display = DisplayStyle.None;
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}
