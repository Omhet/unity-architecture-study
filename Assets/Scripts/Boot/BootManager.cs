namespace Boot
{
    using VContainer.Unity;
    using Infra.SceneManagement;
    using Cysharp.Threading.Tasks;

    public class BootManager : IStartable
    {
        private readonly LoadingScreenController _loadingScreen;

        public BootManager(LoadingScreenController loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }

        public void Start()
        {
            // Instantly transition to Menu once Boot is fully resolved
            LoadMenuAsync().Forget();
        }

        private async UniTaskVoid LoadMenuAsync()
        {
            await _loadingScreen.LoadSceneAsync("Menu");
        }
    }
}
