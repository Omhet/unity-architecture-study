namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Resources.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class ResourceSaveData
    {
        public Dictionary<string, int> Balances { get; set; } = new Dictionary<string, int>();
    }

    public class ResourceSaveModule : ISaveModule
    {
        private readonly ResourceState _state;

        public string Key => "resources";

        public ResourceSaveModule(ResourceState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new ResourceSaveData
            {
                Balances = new Dictionary<string, int>(_state.Balances)
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<ResourceSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
            var data = bundle.GetData<ResourceSaveData>(Key);
            foreach (var entry in data.Balances)
            {
                if (entry.Value < 0)
                {
                    errors.Add($"Resource '{entry.Key}' amount must be >= 0.");
                }
            }
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<ResourceSaveData>(Key);
            _state.Clear();
            foreach (var entry in data.Balances)
            {
                _state.SetAmount(entry.Key, entry.Value);
            }
        }
    }
}
