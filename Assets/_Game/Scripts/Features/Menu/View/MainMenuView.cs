namespace App.Menu.View
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using R3;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        private Button _playButton;
        private readonly Subject<Unit> _onPlayClicked = new Subject<Unit>();

        // Flow layer can subscribe to know when the user wants to play
        public Observable<Unit> OnPlayClicked => _onPlayClicked;

        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null || uiDocument.rootVisualElement == null)
                return;

            var root = uiDocument.rootVisualElement;

            // If a play button already exists (e.g. UXML still assigned), use it.
            _playButton = root.Q<Button>("play-button");

            // If there's no UXML, build the UI in code and apply the USS if provided.
            if (_playButton == null)
            {
                root.Clear();
                if (_styleSheet != null)
                {
                    root.styleSheets.Add(_styleSheet);
                }

                var container = new VisualElement();
                container.AddToClassList("menu-container");

                var title = new Label("CLICKER TOY");
                title.AddToClassList("title-text");

                var playBtn = new Button(HandlePlayClicked)
                {
                    name = "play-button",
                    text = "PLAY GAME"
                };
                playBtn.AddToClassList("play-button");

                container.Add(title);
                container.Add(playBtn);
                root.Add(container);

                _playButton = playBtn;
            }

            if (_playButton != null)
            {
                _playButton.clicked += HandlePlayClicked;
            }
        }

        private void OnDisable()
        {
            if (_playButton != null)
            {
                _playButton.clicked -= HandlePlayClicked;
            }
            _onPlayClicked.OnCompleted();
        }

        private void HandlePlayClicked()
        {
            _onPlayClicked.OnNext(Unit.Default);
        }
    }
}