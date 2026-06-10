namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Progression.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class ProgressionConfigModule : IConfigModule
    {
        private readonly ProgressionRegistry _progressionRegistry;
        private readonly ProgressionState _progressionState;

        public string Key => "progression";

        public ProgressionConfigModule(ProgressionRegistry progressionRegistry, ProgressionState progressionState)
        {
            _progressionRegistry = progressionRegistry;
            _progressionState = progressionState;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<ProgressionCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var config = bundle.GetConfig<ProgressionCatalogConfig>(Key);
            if (config?.Levels == null)
            {
                errors.Add("Missing progression catalog.");
                return;
            }

            var seen = new HashSet<int>();
            for (int i = 0; i < config.Levels.Length; i++)
            {
                var entry = config.Levels[i];
                if (seen.Contains(entry.Level))
                {
                    errors.Add("Duplicate progression level: " + entry.Level);
                }
                else
                {
                    seen.Add(entry.Level);
                }

                if (entry.NextLevelXp < 0)
                {
                    errors.Add("Invalid NextLevelXp for level " + entry.Level);
                }
            }
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<ProgressionCatalogConfig>(Key);
            _progressionRegistry.Load(config?.Levels);

            // initialize state's next-level xp based on first level
            // TODO: Move it into a flow handler
            _progressionState.NextLevelXp.Value = _progressionRegistry.GetNextLevelXpForLevel(1);
        }
    }
}
