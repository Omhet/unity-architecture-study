namespace App.Boot
{
    using App.Infra.Saving;
    using App.Infra.SceneManagement;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using MessagePipe;

    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private SceneLoader _loadingScreenPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            builder.Register<SaveService>(Lifetime.Singleton);

            if (_loadingScreenPrefab != null)
            {
                builder.RegisterComponentInNewPrefab(_loadingScreenPrefab, Lifetime.Singleton);
            }

            builder.RegisterEntryPoint<BootManager>();
        }
    }
}
