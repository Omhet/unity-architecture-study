namespace App.Quests.Core
{
    using System;

    [Serializable]
    public class QuestDefinition
    {
        public string Id;
        public int XpReward;
        public ConditionData ConditionData;
    }

    [Serializable]
    public class QuestCatalogConfig
    {
        public QuestDefinition[] Quests;
    }
}
