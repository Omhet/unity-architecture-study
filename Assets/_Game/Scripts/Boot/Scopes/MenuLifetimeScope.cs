namespace App.Boot
{
    using App.Menu.Flow;
    using App.Menu.View;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter.VContainer;

    public class MenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private MainMenuView _mainMenuView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuPresenter>();

            builder.RegisterComponent(_mainMenuView);

            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<PlayGameEventHandler>();
            });
        }
    }
}