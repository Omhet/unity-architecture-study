namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Progression.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class ProgressionSaveData
    {
        public int Level { get; set; }
        public int Xp { get; set; }
        public int NextLevelXp { get; set; }
    }

    public class ProgressionSaveModule : ISaveModule
    {
        private readonly ProgressionState _state;

        public string Key => "progression";

        public ProgressionSaveModule(ProgressionState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new ProgressionSaveData
            {
                Level = _state.Level.Value,
                Xp = _state.Xp.Value,
                NextLevelXp = _state.NextLevelXp.Value
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<ProgressionSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
            var data = bundle.GetData<ProgressionSaveData>(Key);
            if (data.Level < 1)
            {
                errors.Add("Progression level must be >= 1.");
            }
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<ProgressionSaveData>(Key);
            _state.Level.Value = data.Level;
            _state.Xp.Value = data.Xp;
            _state.NextLevelXp.Value = data.NextLevelXp;
        }
    }
}
