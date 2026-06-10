namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Generators.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class GeneratorSaveData
    {
        public List<string> OwnedGeneratorIds { get; set; } = new List<string>();
    }

    public class GeneratorSaveModule : ISaveModule
    {
        private readonly GeneratorState _state;

        public string Key => "generators";

        public GeneratorSaveModule(GeneratorState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new GeneratorSaveData
            {
                OwnedGeneratorIds = new List<string>(_state.PlayerOwnedGeneratorIds)
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<GeneratorSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<GeneratorSaveData>(Key);
            _state.PlayerOwnedGeneratorIds.Clear();
            foreach (var generatorId in data.OwnedGeneratorIds)
            {
                _state.PlayerOwnedGeneratorIds.Add(generatorId);
            }
        }
    }
}
