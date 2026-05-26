namespace App.Menu.View
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;

        private Button _playButton;

        public event Action OnPlayClicked;

        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null || uiDocument.rootVisualElement == null)
                return;

            var root = uiDocument.rootVisualElement;

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

            _playButton.clicked += HandlePlayClicked;

            Debug.Log("MainMenuView initialized");
        }

        private void HandlePlayClicked()
        {
            OnPlayClicked?.Invoke();
        }
    }
}