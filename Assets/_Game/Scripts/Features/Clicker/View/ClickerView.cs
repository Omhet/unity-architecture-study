namespace App.Clicker.View
{
    using App.Flow.Events;
    using App.View;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;
    using VitalRouter;

    [RequireComponent(typeof(UIDocument))]
    public class ClickerView : GameplayViewBase
    {
        private ICommandPublisher _publisher;

        [Inject]
        public void Construct(ICommandPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override void BuildView()
        {
            var root = PrepareRoot();
            if (root == null)
            {
                return;
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
