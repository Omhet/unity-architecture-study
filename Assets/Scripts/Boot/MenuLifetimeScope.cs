namespace Boot
{
    using Flow.Menu;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using View.Menu;

    public class MenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private MainMenuView _mainMenuView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_mainMenuView);

            builder.RegisterEntryPoint<MenuController>();
        }
    }
}