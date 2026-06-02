namespace App.GameConfig.Core
{
    using System;

    [Serializable]
    public class GameConfigManifest
    {
        public int SchemaVersion;
        public ConfigCatalogEntry[] Catalogs;
    }

    [Serializable]
    public class ConfigCatalogEntry
    {
        public string Key;
        public string Address;
        public int SchemaVersion;
    }
}
