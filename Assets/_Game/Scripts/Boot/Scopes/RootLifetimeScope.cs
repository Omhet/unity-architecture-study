namespace App.Boot
{
    using App.Economy.Core;
    using App.Flow.Handlers;
    using App.Generators.Core;
    using App.Resources.Core;
    using App.Systems.Configuration;
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

        private readonly string _manifestAddress = "config/game_manifest";

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

            builder.Register<GeneratorRegistry>(Lifetime.Singleton);
            builder.Register<PlayerGeneratorModel>(Lifetime.Singleton);
            builder.Register<GeneratorService>(Lifetime.Singleton);

            builder.Register<ResourceRegistry>(Lifetime.Singleton);
            builder.Register<ResourceState>(Lifetime.Singleton);


            builder.RegisterInstance(new GameConfigBootstrapOptions
            {
                ManifestAddress = _manifestAddress
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
