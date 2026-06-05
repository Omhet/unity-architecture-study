namespace App.Flow.Handlers
{
    using System.Collections.Generic;
    using App.Flow.Events;
    using App.Quests.Core;
    using R3;
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
            // Initialize active quests based on registry and set up evaluators
            for (int i = 0; i < _questRegistry.Count; i++)
            {
                var definition = _questRegistry[i];
                if (definition == null)
                {
                    continue;
                }

                var activeQuest = new ActiveQuest(definition.Id, definition.XpReward);
                _questState.ActiveQuests.Add(activeQuest);

                var evaluator = _questService.CreateEvaluator(definition.ConditionData);
                if (evaluator != null)
                {
                    var subscription = evaluator.Observe().Subscribe(claimed =>
                    {
                        activeQuest.IsClaimable.Value = claimed;
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
