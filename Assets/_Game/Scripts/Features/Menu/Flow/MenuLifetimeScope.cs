namespace App.Menu.View
{
    using App.Menu.Flow;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

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