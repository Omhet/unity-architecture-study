namespace App.Systems.Scene
{
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        private UIDocument _uiDocument;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            BuildView();
            Hide();
        }

        private void BuildView()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                return;
            }

            var root = _uiDocument.rootVisualElement;

            root.Clear();

            if (_styleSheet != null)
            {
                root.styleSheets.Add(_styleSheet);
            }

            var loadingContainer = new VisualElement { name = "loading-container" };
            loadingContainer.AddToClassList("loading-container");

            var loadingLabel = new Label("LOADING...") { name = "loading-text" };
            loadingLabel.AddToClassList("loading-text");

            loadingContainer.Add(loadingLabel);
            root.Add(loadingContainer);
        }

        public void Show()
        {
            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                var root = _uiDocument.rootVisualElement;
                root.style.display = DisplayStyle.Flex;
            }
        }

        public void Hide()
        {
            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                var root = _uiDocument.rootVisualElement;
                root.style.display = DisplayStyle.None;
            }
        }
    }
}
