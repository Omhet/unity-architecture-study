namespace App.Boot
{
    using App.Infra.Saving;
    using App.Infra.SceneManagement;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private SceneLoader _loadingScreenPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SaveService>(Lifetime.Singleton);

            if (_loadingScreenPrefab != null)
            {
                builder.RegisterComponentInNewPrefab(_loadingScreenPrefab, Lifetime.Singleton);
            }
        }
    }
}
