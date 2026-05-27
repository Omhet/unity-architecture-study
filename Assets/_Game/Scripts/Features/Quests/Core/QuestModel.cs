namespace App.Quests.Core
{
    using ObservableCollections;

    public class QuestModel
    {
        public ObservableDictionary<string, int> ProgressByQuestId { get; } = new ObservableDictionary<string, int>();
        public ObservableDictionary<string, bool> CompletedByQuestId { get; } = new ObservableDictionary<string, bool>();
    }
}
