namespace App.Quests.Core
{
    using System.Collections.Generic;

    public class QuestRegistry
    {
        private readonly List<QuestDefinition> _quests = new List<QuestDefinition>();

        public void Load(QuestCatalogConfig config)
        {
            _quests.Clear();

            if (config?.Quests == null)
            {
                return;
            }

            for (int i = 0; i < config.Quests.Length; i++)
            {
                var quest = config.Quests[i];
                if (quest == null || string.IsNullOrWhiteSpace(quest.Id))
                {
                    continue;
                }

                _quests.Add(quest);
            }
        }

        public bool TryGetById(string questId, out QuestDefinition quest)
        {
            quest = null;
            if (string.IsNullOrWhiteSpace(questId))
            {
                return false;
            }

            for (int i = 0; i < _quests.Count; i++)
            {
                var current = _quests[i];
                if (current != null && current.Id == questId)
                {
                    quest = current;
                    return true;
                }
            }

            return false;
        }

        public int Count => _quests.Count;

        public QuestDefinition this[int index] => _quests[index];
    }
}
