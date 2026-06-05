namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Talents.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;
    using App.Boot.Utility;

    public class TalentConfigModule : IConfigModule
    {
        private readonly TalentRegistry _talentRegistry;

        public string Key => "talents";

        public TalentConfigModule(TalentRegistry talentRegistry)
        {
            _talentRegistry = talentRegistry;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<TalentCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var config = bundle.GetConfig<TalentCatalogConfig>(Key);
            if (config?.Talents == null)
            {
                errors.Add("Missing talents catalog.");
                return;
            }

            ConfigValidationHelper.ValidateUniqueIds(config.Talents, x => x?.Id, "talents", errors);
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<TalentCatalogConfig>(Key);
            _talentRegistry.Load(config.Talents);
        }
    }
}
