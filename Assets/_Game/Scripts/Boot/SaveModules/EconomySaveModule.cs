#nullable enable

namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Economy.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class EconomySaveModule : ISaveModule
    {
        private readonly EconomyState _state;

        public string Key => "economy";

        public EconomySaveModule(EconomyState state)
        {
            _state = state;
        }

        public object? Serialize()
        {
            return new { balance = _state.Balance.Value };
        }

        public void Deserialize(object data)
        {
            var obj = (JObject)data;
            _state.Balance.Value = obj["balance"]!.Value<int>();
        }

        public void Validate(object data, List<string> errors)
        {
            var obj = (JObject)data;
            var balanceToken = obj["balance"];
            if (balanceToken != null && balanceToken.Type == JTokenType.Integer && balanceToken.Value<int>() < 0)
            {
                errors.Add("Economy balance must be >= 0.");
            }
        }
    }
}