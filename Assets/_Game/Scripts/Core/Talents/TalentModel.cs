namespace App.Talents.Core
{
    using ObservableCollections;

    public class TalentModel
    {
        public ObservableDictionary<string, bool> UnlockedTalents { get; } = new ObservableDictionary<string, bool>();

        public bool IsUnlocked(string talentId)
        {
            return !string.IsNullOrWhiteSpace(talentId)
                && UnlockedTalents.TryGetValue(talentId, out bool unlocked)
                && unlocked;
        }
    }
}
