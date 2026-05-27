namespace App.Systems.Configuration
{
    using Cysharp.Threading.Tasks;

    public class GameConfigInitializationSystem
    {
        private readonly GameConfigLoader _loader;
        private readonly GameConfigValidator _validator;
        private readonly GameConfigHydrator _hydrator;
        private readonly GameConfigBootstrapOptions _options;

        public GameConfigInitializationSystem(
            GameConfigLoader loader,
            GameConfigValidator validator,
            GameConfigHydrator hydrator,
            GameConfigBootstrapOptions options)
        {
            _loader = loader;
            _validator = validator;
            _hydrator = hydrator;
            _options = options;
        }

        public async UniTask InitializeAsync()
        {
            var bundle = await _loader.LoadAsync(_options.ManifestAddress);
            _validator.ValidateOrThrow(bundle);
            _hydrator.Hydrate(bundle);
        }
    }
}
