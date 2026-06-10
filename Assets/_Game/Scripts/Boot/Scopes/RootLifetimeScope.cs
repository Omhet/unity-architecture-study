namespace App.Boot
{
    using App.Boot.ConfigModules;
    using App.Boot.SaveModules;
    using App.Craft.Core;
    using App.Economy.Core;
    using App.Flow.Handlers;
    using App.Generators.Core;
    using App.Orders.Core;
    using App.Products.Core;
    using App.Quests.Core;
    using App.Recipes.Core;
    using App.Resources.Core;
    using App.Shop.Core;
    using App.Progression.Core;
    using App.Systems.Configuration;
    using App.Systems.Saving.Migrations;
    using App.Systems.Saving.Options;
    using App.Systems.Saving.Orchestration;
    using App.Systems.Saving.Storage;
    using App.Systems.Scene;
    using App.Talents.Core;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter;
    using VitalRouter.VContainer;
    using App.Systems.Saving.Modules;

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

            // Scene system
            builder.Register<SceneLoadSystem>(Lifetime.Singleton);

            // Save storage - conditional by platform
#if UNITY_WEBGL
                builder.Register<LocalStorageSaveStorage>(Lifetime.Singleton).As<ISaveStorage>();
#else
            builder.Register<FileSystemSaveStorage>(Lifetime.Singleton).As<ISaveStorage>();
#endif

            // Save bootstrap options
            builder.RegisterInstance(new SaveBootstrapOptions { SlotCount = 4 });

            // Migration chain builder - collects all ISaveMigration instances from DI
            builder.Register<MigrationChainBuilder>(Lifetime.Singleton);

            // Domain save modules - registered as ISaveModule for SaveLoadSystem discovery
            builder.Register<EconomySaveModule>(Lifetime.Singleton).As<ISaveModule>();
            builder.Register<ResourceSaveModule>(Lifetime.Singleton).As<ISaveModule>();

            // Save orchestration - SaveLoadSystem and SlotManager receive dependencies via DI
            builder.Register<SaveLoadSystem>(Lifetime.Singleton);
            builder.Register<SlotManager>(Lifetime.Singleton);

            builder.Register<EconomyState>(Lifetime.Singleton);
            builder.Register<EconomyService>(Lifetime.Singleton);

            builder.Register<GeneratorRegistry>(Lifetime.Singleton);
            builder.Register<GeneratorState>(Lifetime.Singleton);
            builder.Register<GeneratorService>(Lifetime.Singleton);

            builder.Register<ResourceRegistry>(Lifetime.Singleton);
            builder.Register<ResourceState>(Lifetime.Singleton);

            builder.Register<ProductRegistry>(Lifetime.Singleton);
            builder.Register<ProductState>(Lifetime.Singleton);

            builder.Register<RecipeRegistry>(Lifetime.Singleton);
            builder.Register<RecipeState>(Lifetime.Singleton);

            builder.Register<CraftService>(Lifetime.Singleton);

            builder.Register<OrderState>(Lifetime.Singleton);
            builder.Register<OrderService>(Lifetime.Singleton);

            builder.Register<ShopRegistry>(Lifetime.Singleton);
            builder.Register<ShopProgressionRegistry>(Lifetime.Singleton);
            builder.Register<ShopState>(Lifetime.Singleton);
            builder.Register<ShopService>(Lifetime.Singleton);

            builder.Register<ProgressionRegistry>(Lifetime.Singleton);
            builder.Register<ProgressionState>(Lifetime.Singleton);
            builder.Register<ProgressionService>(Lifetime.Singleton);

            builder.Register<QuestRegistry>(Lifetime.Singleton);
            builder.Register<QuestState>(Lifetime.Singleton);
            builder.Register<QuestService>(Lifetime.Singleton);

            builder.Register<TalentRegistry>(Lifetime.Singleton);
            builder.Register<TalentState>(Lifetime.Singleton);
            builder.Register<TalentService>(Lifetime.Singleton);

            builder.RegisterInstance(new GameConfigBootstrapOptions
            {
                ManifestAddress = _manifestAddress
            });

            // Register config modules as IConfigModule
            builder.Register<ResourceConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<GeneratorConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<ProductConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<RecipeConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<ShopConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<ProgressionConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<QuestConfigModule>(Lifetime.Singleton).As<IConfigModule>();
            builder.Register<TalentConfigModule>(Lifetime.Singleton).As<IConfigModule>();

            // Register orchestrators - they receive IEnumerable<IConfigModule> from the container
            builder.Register<GameConfigLoader>(Lifetime.Singleton);
            builder.Register<GameConfigValidator>(Lifetime.Singleton);
            builder.Register<GameConfigHydrator>(Lifetime.Singleton);
            builder.Register<GameConfigInitializationSystem>(Lifetime.Singleton);

            // Save on quit safety net (fire-and-forget save when Application.quitting fires)
            builder.Register<SaveOnQuitSystem>(Lifetime.Singleton);

            if (_loadingScreenViewPrefab != null)
            {
                builder.RegisterComponentInNewPrefab(_loadingScreenViewPrefab, Lifetime.Singleton);
            }
        }
    }
}
