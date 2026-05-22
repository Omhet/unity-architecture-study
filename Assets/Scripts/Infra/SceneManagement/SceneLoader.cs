namespace Infra.SceneManagement
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;
    using Cysharp.Threading.Tasks;

    [RequireComponent(typeof(UIDocument))]
    public class SceneLoader : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            _uiDocument = GetComponent<UIDocument>();

            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            }
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            }

            // Optional future step: Trigger fade-in here

            await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask();

            // Optional future step: Trigger fade-out here

            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            }
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}
