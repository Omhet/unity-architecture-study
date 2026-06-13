namespace App.Quests.Core
{
    using System.Collections.Generic;

    public class QuestState
    {
        public Dictionary<string, QuestProgressData> ProgressMap { get; } = new Dictionary<string, QuestProgressData>();
    }
}
