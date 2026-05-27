namespace App.Generators.Core
{
    using System.Collections.Generic;

    public class GeneratorModel
    {
        public List<GeneratorState> Generators { get; } = new List<GeneratorState>();

        public bool TryGetById(string generatorId, out GeneratorState generator)
        {
            generator = null;
            if (string.IsNullOrWhiteSpace(generatorId))
            {
                return false;
            }

            for (int i = 0; i < Generators.Count; i++)
            {
                var current = Generators[i];
                if (current != null && current.Id == generatorId)
                {
                    generator = current;
                    return true;
                }
            }

            return false;
        }

        public class GeneratorState
        {
            public string Id;
            public string DisplayName;
            public string ResourceId;
            public int AmountPerClick;
        }
    }
}