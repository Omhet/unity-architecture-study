namespace App.Menu.Flow
{
    using System;
    using App.Menu.View;
    using VContainer.Unity;
    using VitalRouter;

    public class MainMenuPresenter : IInitializable, IDisposable
    {
        private readonly MainMenuView _view;
        private readonly ICommandPublisher _publisher;

        public MainMenuPresenter(MainMenuView view, ICommandPublisher publisher)
        {
            _view = view;
            _publisher = publisher;
        }

        public void Initialize()
        {
            _view.OnPlayClicked += HandlePlayClicked;
        }

        private void HandlePlayClicked()
        {
            _publisher.PublishAsync(new PlayGameEvent());
        }

        public void Dispose()
        {
            _view.OnPlayClicked -= HandlePlayClicked;
        }
    }
}