namespace App.Clicker.View
{
    using App.Flow.Events;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;
    using VitalRouter;

    [RequireComponent(typeof(UIDocument))]
    public class ClickerView : MonoBehaviour
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

            var root = _uiDocument.rootVisualElement;
            root.Clear();

            if (_styleSheet != null)
            {
                root.styleSheets.Add(_styleSheet);
            }

            var container = new VisualElement();
            container.AddToClassList("clicker-container");

            var clickButton = new Button(HandleClick)
            {
                name = "clicker-button",
                text = "Click +$1"
            };
            clickButton.AddToClassList("clicker-button");

            container.Add(clickButton);
            root.Add(container);
        }

        private void HandleClick()
        {
            if (_publisher == null)
            {
                return;
            }

            _publisher.PublishAsync(new ClickerClickEvent());
        }
    }
}
