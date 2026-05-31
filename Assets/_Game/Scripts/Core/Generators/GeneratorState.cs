namespace App.Generators.Core
{
    using ObservableCollections;

    public class GeneratorState
    {
        public ObservableList<string> PlayerOwnedGeneratorIds { get; } = new ObservableList<string>();

        public bool IsPlayerOwned(string generatorId)
        {
            if (string.IsNullOrWhiteSpace(generatorId))
            {
                return false;
            }

            for (int i = 0; i < PlayerOwnedGeneratorIds.Count; i++)
            {
                if (PlayerOwnedGeneratorIds[i] == generatorId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}