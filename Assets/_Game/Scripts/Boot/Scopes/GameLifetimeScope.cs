namespace App.Boot
{
    using App.Economy.View;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private EconomyView _economyView;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_economyView != null)
            {
                builder.RegisterComponent(_economyView);
            }
        }
    }
}
