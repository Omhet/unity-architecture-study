namespace App.Root.View
{
    using App.Root.Flow;
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
