namespace App.Boot
{
    using App.Economy.Core;
    using App.Flow.Handlers;
    using App.Generators.Core;
    using App.Shop.Core;
    using App.Orders.Core;
    using App.Products.Core;
    using App.Progression.Core;
    using App.Quests.Core;
    using App.Resources.Core;
    using App.Systems.Configuration;
    using App.Talents.Core;
    using App.Systems.Saving;
    using App.Systems.Scene;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter;
    using VitalRouter.VContainer;

    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private LoadingScreenView _loadingScreenViewPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(Router.Default).AsImplementedInterfaces();

            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<SceneFlowHandler>();
            });

            builder.Register<SaveSystem>(Lifetime.Singleton);
            builder.Register<SceneLoadSystem>(Lifetime.Singleton);

            builder.Register<EconomyModel>(Lifetime.Singleton);
            builder.Register<EconomyService>(Lifetime.Singleton);
            builder.Register<GeneratorModel>(Lifetime.Singleton);
            builder.Register<PlayerGeneratorModel>(Lifetime.Singleton);
            builder.Register<GeneratorService>(Lifetime.Singleton);
            builder.Register<ShopModel>(Lifetime.Singleton);
            builder.Register<ResourceModel>(Lifetime.Singleton);
            builder.Register<ProductInventoryModel>(Lifetime.Singleton);
            builder.Register<ProgressionModel>(Lifetime.Singleton);
            builder.Register<TalentModel>(Lifetime.Singleton);
            builder.Register<OrderModel>(Lifetime.Singleton);
            builder.Register<QuestModel>(Lifetime.Singleton);


            builder.RegisterInstance(new GameConfigBootstrapOptions
            {
                ManifestAddress = "config/game_manifest"
            });

            builder.Register<GameConfigLoader>(Lifetime.Singleton);
            builder.Register<GameConfigValidator>(Lifetime.Singleton);
            builder.Register<GameConfigHydrator>(Lifetime.Singleton);
            builder.Register<GameConfigInitializationSystem>(Lifetime.Singleton);

            if (_loadingScreenViewPrefab != null)
            {
                builder.RegisterComponentInNewPrefab(_loadingScreenViewPrefab, Lifetime.Singleton);
            }
        }
    }
}
