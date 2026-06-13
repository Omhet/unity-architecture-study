namespace App.Flow.Handlers
{
    using System.Collections.Generic;
    using App.Flow.Events;
    using App.Quests.Core;
    using R3;
    using UnityEngine;
    using VitalRouter;

    [Routes]
    public partial class QuestFlowHandler
    {
        private readonly QuestService _questService;
        private readonly QuestRegistry _questRegistry;
        private readonly QuestState _questState;

        private readonly Dictionary<string, System.IDisposable> _evaluatorSubscriptions = new Dictionary<string, System.IDisposable>();

        public QuestFlowHandler(QuestService questService, QuestRegistry questRegistry, QuestState questState)
        {
            _questService = questService;
            _questRegistry = questRegistry;
            _questState = questState;
        }


        [Route]
        void On(StartGameEvent _)
        {
            // Initialize progress entries from registry and set up evaluators
            for (int i = 0; i < _questRegistry.Count; i++)
            {
                var definition = _questRegistry[i];
                if (definition == null)
                {
                    continue;
                }

                // Ensure each quest has a progress entry
                if (!_questState.ProgressMap.ContainsKey(definition.Id))
                {
                    _questState.ProgressMap[definition.Id] = new QuestProgressData();
                }

                var progress = _questState.ProgressMap[definition.Id];

                // Skip evaluator creation for already-completed quests
                if (progress.IsCompleted.Value)
                {
                    continue;
                }

                Debug.Log($"Initialized progress for quest '{definition.Id}': IsCompleted={progress.IsCompleted.Value}, IsClaimable={progress.IsClaimable.Value}");

                var evaluator = _questService.CreateEvaluator(definition.ConditionData);
                if (evaluator != null)
                {
                    Debug.Log($"Created evaluator for quest '{definition.Id}' and subscribing to changes.");
                    var subscription = evaluator.Observe().Subscribe(claimed =>
                    {
                        Debug.Log($"Quest '{definition.Id}' condition evaluated: claimed={claimed}");
                        progress.IsClaimable.Value = claimed;
                    });
                    _evaluatorSubscriptions[definition.Id] = subscription;
                }
            }
        }

        [Route]
        void On(ClaimQuestEvent command)
        {
            _questService.Claim(command.QuestId);

            // Dispose evaluator subscription for completed quest
            if (_evaluatorSubscriptions.TryGetValue(command.QuestId, out var subscription))
            {
                subscription.Dispose();
                _evaluatorSubscriptions.Remove(command.QuestId);
            }
        }
    }
}
