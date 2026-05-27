namespace App.Boot
{
    using App.Clicker.View;
    using App.Economy.Core;
    using App.Economy.View;
    using App.Orders.Core;
    using App.Products.Core;
    using App.Progression.Core;
    using App.Quests.Core;
    using App.Resources.Core;
    using App.Shop.Core;
    using App.Systems.Configuration;
    using App.Talents.Core;
    using App.Unlocks.Core;
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

            builder.Register<EconomyModel>(Lifetime.Singleton);
            builder.Register<EconomyService>(Lifetime.Singleton);
            builder.Register<ShopModel>(Lifetime.Singleton);
            builder.Register<ResourceModel>(Lifetime.Singleton);
            builder.Register<ProductInventoryModel>(Lifetime.Singleton);
            builder.Register<ProgressionModel>(Lifetime.Singleton);
            builder.Register<TalentModel>(Lifetime.Singleton);
            builder.Register<UnlockModel>(Lifetime.Singleton);
            builder.Register<OrderModel>(Lifetime.Singleton);
            builder.Register<QuestModel>(Lifetime.Singleton);

            builder.RegisterInstance(new GameConfigBootstrapOptions
            {
                ManifestAddress = _gameConfigManifestAddress
            });

            builder.Register<GameConfigLoader>(Lifetime.Singleton);
            builder.Register<GameConfigValidator>(Lifetime.Singleton);
            builder.Register<GameConfigHydrator>(Lifetime.Singleton);
            builder.Register<GameConfigInitializationSystem>(Lifetime.Singleton);

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
