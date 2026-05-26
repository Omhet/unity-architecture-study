namespace App.Boot
{
    using App.Systems.Saving;
    using App.Systems.Scene;
    using App.Shared.Flow;
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
                routing.Map<PlayGameEventHandler>();
            });

            builder.Register<SaveSystem>(Lifetime.Singleton);
            builder.Register<SceneLoadSystem>(Lifetime.Singleton);

            if (_loadingScreenViewPrefab != null)
            {
                builder.RegisterComponentInNewPrefab(_loadingScreenViewPrefab, Lifetime.Singleton);
            }
        }
    }
}
