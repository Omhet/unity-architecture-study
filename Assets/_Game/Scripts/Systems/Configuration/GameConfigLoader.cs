namespace App.Systems.Configuration
{
    using System;
    using System.Collections.Generic;
    using App.GameConfig.Core;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class GameConfigLoader
    {
        private readonly IEnumerable<IConfigModule> _modules;

        public GameConfigLoader(IEnumerable<IConfigModule> modules)
        {
            _modules = modules;
        }

        public async UniTask<GameCatalogBundle> LoadAsync(string manifestAddress)
        {
            if (string.IsNullOrWhiteSpace(manifestAddress))
            {
                throw new InvalidOperationException("Manifest address is required for config loading.");
            }

            TextAsset manifestAsset = await Addressables.LoadAssetAsync<TextAsset>(manifestAddress).ToUniTask();
            if (manifestAsset == null)
            {
                throw new InvalidOperationException("Failed to load config manifest at address: " + manifestAddress);
            }

            var manifest = JsonConvert.DeserializeObject<GameConfigManifest>(manifestAsset.text);
            if (manifest == null)
            {
                throw new InvalidOperationException("Failed to parse game config manifest.");
            }

            var bundle = new GameCatalogBundle
            {
                Manifest = manifest
            };

            if (manifest.Catalogs == null)
            {
                return bundle;
            }

            foreach (var catalog in manifest.Catalogs)
            {
                if (catalog == null || string.IsNullOrWhiteSpace(catalog.Key) || string.IsNullOrWhiteSpace(catalog.Address))
                {
                    continue;
                }

                TextAsset catalogAsset = await Addressables.LoadAssetAsync<TextAsset>(catalog.Address).ToUniTask();
                if (catalogAsset == null)
                {
                    throw new InvalidOperationException("Failed to load catalog: " + catalog.Key + " at address: " + catalog.Address);
                }

                DeserializeCatalog(catalog.Key, catalogAsset.text, bundle);
            }

            return bundle;
        }

        private void DeserializeCatalog(string catalogKey, string json, GameCatalogBundle bundle)
        {
            foreach (var module in _modules)
            {
                if (module.Key == catalogKey)
                {
                    module.Deserialize(json, bundle);
                    return;
                }
            }

            Debug.LogWarning("Unknown catalog key in manifest: " + catalogKey);
        }
    }
}
