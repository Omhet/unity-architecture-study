namespace App.Generators.Core
{
    using ObservableCollections;

    public class GeneratorState
    {
        public ObservableList<string> OwnedGeneratorIds { get; } = new ObservableList<string>();

        public bool IsOwned(string generatorId)
        {
            if (string.IsNullOrWhiteSpace(generatorId))
            {
                return false;
            }

            for (int i = 0; i < OwnedGeneratorIds.Count; i++)
            {
                if (OwnedGeneratorIds[i] == generatorId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}