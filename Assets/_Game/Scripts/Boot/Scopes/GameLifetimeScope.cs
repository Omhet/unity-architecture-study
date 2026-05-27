namespace App.Boot
{
    using App.Economy.View;
    using App.Hud.View;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private EconomyView _economyView;
        [SerializeField] private HudShellView _hudShellView;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_economyView != null)
            {
                builder.RegisterComponent(_economyView);
            }

            if (_hudShellView != null)
            {
                builder.RegisterComponent(_hudShellView);
            }
        }
    }
}
