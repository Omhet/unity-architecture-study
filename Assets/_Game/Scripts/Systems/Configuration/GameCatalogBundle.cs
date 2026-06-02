namespace App.Systems.Configuration
{
    using System.Collections.Generic;
    using App.GameConfig.Core;

    public class GameCatalogBundle
    {
        public GameConfigManifest Manifest;

        private readonly Dictionary<string, object> _configs = new Dictionary<string, object>();

        public void SetConfig<T>(string key, T config)
        {
            _configs[key] = config;
        }

        public T GetConfig<T>(string key)
        {
            return (T)_configs[key];
        }
    }
}
