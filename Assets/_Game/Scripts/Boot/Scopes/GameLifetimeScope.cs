namespace App.Boot
{
    using App.Clicker.View;
    using App.Economy.Core;
    using App.Economy.View;
    using App.Shared.Flow;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter.VContainer;

    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ClickerView _clickerView;
        [SerializeField] private EconomyView _economyView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<ClickerClickEventHandler>();
            });

            builder.Register<EconomyModel>(Lifetime.Singleton);
            builder.Register<EconomyService>(Lifetime.Singleton);

            if (_clickerView != null)
            {
                builder.RegisterComponent(_clickerView);
            }

            if (_economyView != null)
            {
                builder.RegisterComponent(_economyView);
            }
        }
    }
}
