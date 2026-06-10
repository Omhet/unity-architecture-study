namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Systems.Saving.Modules;
    using App.Talents.Core;
    using Newtonsoft.Json.Linq;

    public class TalentSaveData
    {
        public int AvailablePoints { get; set; }
        public Dictionary<string, int> PointsSpent { get; set; } = new Dictionary<string, int>();
    }

    public class TalentSaveModule : ISaveModule
    {
        private readonly TalentState _state;

        public string Key => "talents";

        public TalentSaveModule(TalentState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new TalentSaveData
            {
                AvailablePoints = _state.AvailablePoints.Value,
                PointsSpent = new Dictionary<string, int>(_state.PointsSpent)
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<TalentSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<TalentSaveData>(Key);
            _state.AvailablePoints.Value = data.AvailablePoints;
            _state.PointsSpent.Clear();
            foreach (var entry in data.PointsSpent)
            {
                _state.PointsSpent[entry.Key] = entry.Value;
            }
        }
    }
}
