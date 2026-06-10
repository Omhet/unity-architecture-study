#nullable enable

namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Economy.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class EconomySaveData
    {
        public int Balance { get; set; }
    }

    public class EconomySaveModule : ISaveModule
    {
        private readonly EconomyState _state;

        public string Key => "economy";

        public EconomySaveModule(EconomyState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new EconomySaveData
            {
                Balance = _state.Balance.Value
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<EconomySaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
            var data = bundle.GetData<EconomySaveData>(Key);
            if (data.Balance < 0)
            {
                errors.Add("Economy balance must be >= 0.");
            }
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<EconomySaveData>(Key);
            _state.Balance.Value = data.Balance;
        }
    }
}