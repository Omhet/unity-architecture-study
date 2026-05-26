namespace App.Boot
{
    using App.Menu.Flow;
    using App.Menu.View;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class MenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private MainMenuView _mainMenuView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_mainMenuView);
            builder.Register<MainMenuPresenter>(Lifetime.Scoped);
        }
    }
}