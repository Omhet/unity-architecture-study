namespace App.Unlocks.Core
{
    using ObservableCollections;

    public class UnlockModel
    {
        public ObservableDictionary<string, bool> UnlockStates { get; } = new ObservableDictionary<string, bool>();

        public bool IsUnlocked(string unlockId)
        {
            return !string.IsNullOrWhiteSpace(unlockId)
                && UnlockStates.TryGetValue(unlockId, out bool unlocked)
                && unlocked;
        }
    }
}
