namespace App.Boot
{
    using App.Flow.Handlers;
    using App.Hud.View;
    using App.View;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter.VContainer;

    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private HudShellView _hudShellView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<GeneratorFlowHandler>();
                routing.Map<CraftFlowHandler>();
                routing.Map<OrderFlowHandler>();
                routing.Map<ShopFlowHandler>();
                routing.Map<StartGameFlowHandler>();
            });

            builder.Register<HudSectionFactory>(Lifetime.Scoped);
            builder.RegisterFactory<GameplaySectionDefinition, IGameplaySectionView>(
                container => container.Resolve<HudSectionFactory>().Create,
                Lifetime.Scoped);


            // This flow handler doesn't have event routing for now]
            // It is just subscribes to state on init
            // I guess we need to come up with another approach for these "passive" subscribers
            // Maybe they just subscribe to state and then invoke an event for other "active" flow handlers?
            // TODO: Decide on approach for "passive" flow handlers that don't have event routing, but just subscribe to state and react to changes
            builder.Register<LevelUpFlowHandler>(Lifetime.Scoped);

            if (_hudShellView != null)
            {
                builder.RegisterComponent(_hudShellView);
            }
        }
    }
}
