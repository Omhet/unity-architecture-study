namespace App.Systems.Configuration
{
    using System.Collections.Generic;

    public interface IConfigModule
    {
        string Key { get; }
        void Deserialize(string json, GameCatalogBundle bundle);
        void Validate(GameCatalogBundle bundle, List<string> errors);
        void Hydrate(GameCatalogBundle bundle);
    }
}
