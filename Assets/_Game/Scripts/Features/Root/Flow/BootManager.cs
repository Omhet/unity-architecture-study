namespace App.Root.Flow
{
    using System.Threading;
    using App.Root.View;
    using VContainer.Unity;
    using Cysharp.Threading.Tasks;

    public class BootManager : IAsyncStartable
    {
        private readonly SceneLoader _loadingScreen;

        public BootManager(SceneLoader loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            // Instantly transition to Menu once Boot is fully resolved
            await _loadingScreen.LoadSceneAsync("Menu");
        }
    }
}
