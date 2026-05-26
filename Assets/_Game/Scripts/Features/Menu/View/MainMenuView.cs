namespace App.Menu.View
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        private UIDocument _uiDocument;
        private VisualElement _root;

        public event Action OnPlayClicked;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                return;
            }

            _root = _uiDocument.rootVisualElement;

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
            OnPlayClicked?.Invoke();
        }
    }
}