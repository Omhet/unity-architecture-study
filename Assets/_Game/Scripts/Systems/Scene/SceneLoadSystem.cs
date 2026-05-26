namespace App.Systems.Scene
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;
    using Cysharp.Threading.Tasks;

    [RequireComponent(typeof(UIDocument))]
    public class SceneLoadSystem : MonoBehaviour
    {
        [SerializeField] private StyleSheet _loadingStyleSheet;

        private UIDocument _uiDocument;

        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            _uiDocument = GetComponent<UIDocument>();

            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                var root = _uiDocument.rootVisualElement;

                // If no loading container exists (no UXML), construct it in code
                var existing = root.Q("loading-container");
                if (existing == null)
                {
                    root.Clear();
                    if (_loadingStyleSheet != null)
                    {
                        root.styleSheets.Add(_loadingStyleSheet);
                    }

                    var loadingContainer = new VisualElement { name = "loading-container" };
                    loadingContainer.AddToClassList("loading-container");

                    var loadingLabel = new Label("LOADING...") { name = "loading-text" };
                    loadingLabel.AddToClassList("loading-text");

                    loadingContainer.Add(loadingLabel);
                    root.Add(loadingContainer);
                }

                root.style.display = DisplayStyle.None;
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
