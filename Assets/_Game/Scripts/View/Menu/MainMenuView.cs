namespace App.Menu.View
{
    using App.Flow.Events;
    using App.View;
    using UnityEngine;
    using UnityEngine.UIElements;
    using VContainer;
    using VitalRouter;

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : GameplayViewBase
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
            container.AddToClassList("menu-container");

            var title = new Label("Workshop Game");
            title.AddToClassList("title-text");

            var playButton = new Button(HandlePlayClicked)
            {
                name = "play-button",
                text = "Start Game"
            };
            playButton.AddToClassList("play-button");

            container.Add(title);
            container.Add(playButton);
            root.Add(container);
        }

        private void HandlePlayClicked()
        {
            _publisher.PublishAsync(new LoadGameEvent());
        }
    }
}