namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Products.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;

    public class ProductSaveData
    {
        public Dictionary<string, int> Amounts { get; set; } = new Dictionary<string, int>();
    }

    public class ProductSaveModule : ISaveModule
    {
        private readonly ProductState _state;

        public string Key => "products";

        public ProductSaveModule(ProductState state)
        {
            _state = state;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new ProductSaveData
            {
                Amounts = new Dictionary<string, int>(_state.PlayerOwnedProductAmounts)
            };
            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<ProductSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<ProductSaveData>(Key);
            _state.Clear();
            foreach (var entry in data.Amounts)
            {
                _state.SetAmount(entry.Key, entry.Value);
            }
        }
    }
}
