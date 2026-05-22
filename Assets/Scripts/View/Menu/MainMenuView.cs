namespace View.Menu
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using R3;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : MonoBehaviour
    {
        private Button _playButton;
        private readonly Subject<Unit> _onPlayClicked = new Subject<Unit>();

        // Flow layer can subscribe to know when the user wants to play
        public Observable<Unit> OnPlayClicked => _onPlayClicked;

        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument != null && uiDocument.rootVisualElement != null)
            {
                _playButton = uiDocument.rootVisualElement.Q<Button>("play-button");
                if (_playButton != null)
                {
                    _playButton.clicked += HandlePlayClicked;
                }
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