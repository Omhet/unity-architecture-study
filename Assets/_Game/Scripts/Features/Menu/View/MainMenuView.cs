namespace App.Menu.View
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;
    using VitalRouter;
    using App.Menu.Core;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        private UIDocument _uiDocument;
        private ICommandPublisher _publisher;

        [Inject]
        public void Construct(ICommandPublisher publisher)
        {
            _publisher = publisher;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            BuildView();
        }

        private void BuildView()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                return;
            }

            var _root = _uiDocument.rootVisualElement;

            _root.Clear();

            if (_styleSheet != null)
            {
                _root.styleSheets.Add(_styleSheet);
            }

            var container = new VisualElement();
            container.AddToClassList("menu-container");

            var title = new Label("Clicker Game");
            title.AddToClassList("title-text");

            var playButton = new Button(HandlePlayClicked)
            {
                name = "play-button",
                text = "Start Game"
            };
            playButton.AddToClassList("play-button");

            container.Add(title);
            container.Add(playButton);
            _root.Add(container);
        }

        private void HandlePlayClicked()
        {
            _publisher.PublishAsync(new PlayGameEvent());
        }
    }
}