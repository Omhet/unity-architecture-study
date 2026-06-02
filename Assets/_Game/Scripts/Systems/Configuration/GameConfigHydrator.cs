namespace App.Systems.Configuration
{
    using System.Collections.Generic;

    public class GameConfigHydrator
    {
        private readonly IEnumerable<IConfigModule> _modules;

        public GameConfigHydrator(IEnumerable<IConfigModule> modules)
        {
            _modules = modules;
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            foreach (var module in _modules)
            {
                module.Hydrate(bundle);
            }
        }
    }
}
