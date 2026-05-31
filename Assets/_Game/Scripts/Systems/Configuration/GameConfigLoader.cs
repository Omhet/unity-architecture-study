namespace App.Systems.Configuration
{
    using System;
    using App.GameConfig.Core;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class GameConfigLoader
    {
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

                LoadCatalogIntoBundle(bundle, catalog.Key, catalogAsset.text);
            }

            return bundle;
        }

        private static void LoadCatalogIntoBundle(GameCatalogBundle bundle, string catalogKey, string json)
        {
            switch (catalogKey)
            {
                case "resources":
                    bundle.Resources = JsonConvert.DeserializeObject<ResourceCatalogConfig>(json);
                    return;
                case "generators":
                    bundle.Generators = JsonConvert.DeserializeObject<GeneratorCatalogConfig>(json);
                    return;
                case "products":
                    bundle.Products = JsonConvert.DeserializeObject<ProductCatalogConfig>(json);
                    return;
                default:
                    Debug.LogWarning("Unknown catalog key in manifest: " + catalogKey);
                    return;
            }
        }
    }
}
