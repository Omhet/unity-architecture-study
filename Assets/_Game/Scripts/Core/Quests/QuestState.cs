namespace App.Quests.Core
{
    using ObservableCollections;

    public class QuestState
    {
        public ObservableList<ActiveQuest> ActiveQuests { get; } = new ObservableList<ActiveQuest>();
    }
}
