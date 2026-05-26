namespace App.Boot
{
    using App.Infra.Saving;
    using App.Infra.SceneManagement;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;
    using VitalRouter;

    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private SceneLoader _loadingScreenPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(Router.Default);

            builder.Register<SaveService>(Lifetime.Singleton);

            if (_loadingScreenPrefab != null)
            {
                builder.RegisterComponentInNewPrefab(_loadingScreenPrefab, Lifetime.Singleton);
            }
        }
    }
}
