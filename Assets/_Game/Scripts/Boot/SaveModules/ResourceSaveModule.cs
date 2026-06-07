#nullable enable

namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Resources.Core;
    using App.Systems.Saving.Modules;

    public class ResourceSaveModule : ISaveModule
    {
        private readonly ResourceState _state;

        public string Key => "resources";

        public ResourceSaveModule(ResourceState state)
        {
            _state = state;
        }

        public object? Serialize()
        {
            return new Dictionary<string, int>(_state.Balances);
        }

        public void Deserialize(object data)
        {
            var balances = (Dictionary<string, int>)data;
            foreach (var entry in balances)
            {
                _state.SetAmount(entry.Key, entry.Value);
            }
        }

        public void Validate(object data, List<string> errors)
        {
            var balances = (Dictionary<string, int>)data;
            foreach (var entry in balances)
            {
                if (entry.Value < 0)
                {
                    errors.Add($"Resource '{entry.Key}' amount must be >= 0.");
                }
            }
        }
    }
}
