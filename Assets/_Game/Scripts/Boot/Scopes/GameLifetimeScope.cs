namespace App.Boot
{
    using App.Clicker.View;
    using App.Economy.View;
    using App.Flow.Handlers;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter.VContainer;

    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ClickerView _clickerView;
        [SerializeField] private EconomyView _economyView;
        [SerializeField] private string _gameConfigManifestAddress = "config/game_manifest";

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<ClickerClickEventHandler>();
            });

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
