namespace App.Boot.SaveModules
{
    using System.Collections.Generic;
    using App.Quests.Core;
    using App.Systems.Saving.Modules;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    public class QuestEntrySaveData
    {
        public bool IsCompleted { get; set; }
        public bool IsClaimable { get; set; }
    }

    public class QuestSaveData
    {
        public Dictionary<string, QuestEntrySaveData> Progress { get; set; } = new Dictionary<string, QuestEntrySaveData>();
    }

    public class QuestSaveModule : ISaveModule
    {
        private readonly QuestState _questState;

        public string Key => "quests";

        public QuestSaveModule(QuestState questState)
        {
            _questState = questState;
        }

        public void Serialize(SaveDataBundle bundle)
        {
            var data = new QuestSaveData();

            foreach (var entry in _questState.ProgressMap)
            {
                data.Progress[entry.Key] = new QuestEntrySaveData
                {
                    IsCompleted = entry.Value.IsCompleted.Value,
                    IsClaimable = entry.Value.IsClaimable.Value
                };
            }

            bundle.SetData(Key, data);
        }

        public void Deserialize(JToken section, SaveDataBundle bundle)
        {
            var data = section.ToObject<QuestSaveData>()
                ?? throw new System.InvalidOperationException($"Failed to deserialize '{Key}' save section.");
            bundle.SetData(Key, data);
        }

        public void Validate(SaveDataBundle bundle, List<string> errors)
        {
        }

        public void Apply(SaveDataBundle bundle)
        {
            var data = bundle.GetData<QuestSaveData>(Key);

            foreach (var entry in data.Progress)
            {
                // Create entry if it doesn't exist yet
                if (!_questState.ProgressMap.TryGetValue(entry.Key, out var progress))
                {
                    progress = new QuestProgressData();
                    _questState.ProgressMap[entry.Key] = progress;
                }
                Debug.Log($"Applying saved progress for quest '{entry.Key}': IsCompleted={entry.Value.IsCompleted}, IsClaimable={entry.Value.IsClaimable}");
                progress.IsCompleted.Value = entry.Value.IsCompleted;
                progress.IsClaimable.Value = entry.Value.IsClaimable;
            }
        }
    }
}
