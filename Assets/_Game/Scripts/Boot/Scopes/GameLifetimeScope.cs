namespace App.Boot
{
    using App.Systems.Debug;
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
        [SerializeField] private DebugHelper _debugHelper;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<GeneratorFlowHandler>();
                routing.Map<CraftFlowHandler>();
                routing.Map<OrderFlowHandler>();
                routing.Map<ShopFlowHandler>();
                routing.Map<StartGameFlowHandler>();
                routing.Map<LevelUpFlowHandler>();
            });

            builder.Register<HudSectionFactory>(Lifetime.Scoped);
            builder.RegisterFactory<GameplaySectionDefinition, IGameplaySectionView>(
                container => container.Resolve<HudSectionFactory>().Create,
                Lifetime.Scoped);

            if (_hudShellView != null)
            {
                builder.RegisterComponent(_hudShellView);
            }

            if (_debugHelper != null)
            {
                builder.RegisterComponent(_debugHelper);
            }
        }
    }
}
