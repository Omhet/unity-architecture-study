namespace App.Generators.Core
{
    using System.Collections.Generic;

    public class GeneratorRegistry
    {
        private readonly List<GeneratorDefinition> _generators = new List<GeneratorDefinition>();

        public void Load(GeneratorCatalogConfig config)
        {
            _generators.Clear();

            if (config?.Generators == null)
            {
                return;
            }

            for (int i = 0; i < config.Generators.Length; i++)
            {
                var generator = config.Generators[i];
                if (generator == null || string.IsNullOrWhiteSpace(generator.Id))
                {
                    continue;
                }

                _generators.Add(generator);
            }
        }

        public bool TryGetById(string generatorId, out GeneratorDefinition generator)
        {
            generator = null;
            if (string.IsNullOrWhiteSpace(generatorId))
            {
                return false;
            }

            for (int i = 0; i < _generators.Count; i++)
            {
                var current = _generators[i];
                if (current != null && current.Id == generatorId)
                {
                    generator = current;
                    return true;
                }
            }

            return false;
        }

        public int Count => _generators.Count;

        public GeneratorDefinition this[int index] => _generators[index];
    }
}