namespace App.Hud.View
{
    using System;
    using App.Flow.Events;
    using App.Quests.Core;
    using App.View;
    using R3;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class QuestSectionView : GameplaySectionViewBase
    {
        private readonly QuestState _questState;
        private readonly QuestRegistry _questRegistry;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _progressSubscription;

        public QuestSectionView(
            ICommandPublisher publisher,
            QuestState questState,
            QuestRegistry questRegistry
            )
            : base(new GameplaySectionDefinition("quests", "Quests", 0))
        {
            _questState = questState;
            _questRegistry = questRegistry;
            _publisher = publisher;
        }

        protected override void BuildContent(VisualElement root)
        {
            root.AddToClassList("quests-section");

            var sectionTitle = new Label(Definition.TabTitle);
            sectionTitle.AddToClassList("hud-section-title");
            root.Add(sectionTitle);

            _list = new VisualElement();
            _list.AddToClassList("quests-list");

            root.Add(_list);
        }

        protected override void Bind()
        {
            _progressSubscription?.Dispose();

            if (_questState == null || _questRegistry == null)
            {
                return;
            }

            // Registry is static, so we just rebuild once and subscribe to reactive properties
            RebuildRows();
        }

        protected override void Unbind()
        {
            _progressSubscription?.Dispose();
            _progressSubscription = null;
        }

        private VisualElement BuildQuestCard(QuestDefinition definition)
        {
            var progress = _questState.ProgressMap[definition.Id];

            var row = new VisualElement();
            row.AddToClassList("quest-card");

            var title = new Label(definition.Id);
            title.AddToClassList("quest-title");
            row.Add(title);

            // Show condition description from registry definition
            if (definition.ConditionData != null)
            {
                var conditionLabel = new Label(FormatCondition(definition.ConditionData));
                conditionLabel.AddToClassList("quest-condition");
                row.Add(conditionLabel);
            }

            var rewardLabel = new Label($"XP: +{definition.XpReward}");
            rewardLabel.AddToClassList("quest-reward");
            row.Add(rewardLabel);

            // Subscribe to IsClaimable and IsCompleted for reactive UI updates
            var claimableSub = progress.IsClaimable.Subscribe(_ => UpdateCardState(row, definition.Id));
            var completedSub = progress.IsCompleted.Subscribe(_ => UpdateCardState(row, definition.Id));
            TrackDisposable(claimableSub);
            TrackDisposable(completedSub);

            // Initial state
            UpdateCardState(row, definition.Id);

            return row;
        }

        private void UpdateCardState(VisualElement card, string questId)
        {
            var progress = _questState.ProgressMap[questId];

            // Remove old action elements (button or checkmark)
            var oldAction = card.Q<VisualElement>("quest-action");
            if (oldAction != null)
            {
                oldAction.RemoveFromHierarchy();
            }

            if (progress.IsCompleted.Value)
            {
                // Show checkmark for completed quests
                var checkmark = new Label("✓");
                checkmark.AddToClassList("quest-checkmark");
                checkmark.name = "quest-action";
                card.Add(checkmark);
            }
            else
            {
                // Show claim button (enabled when claimable)
                var claimButton = new Button(() => HandleClaimClicked(questId))
                {
                    text = "Claim"
                };
                claimButton.AddToClassList("quest-button");
                claimButton.name = "quest-action";
                claimButton.SetEnabled(progress.IsClaimable.Value);
                card.Add(claimButton);
            }
        }

        private void HandleClaimClicked(string questId)
        {
            _publisher.PublishAsync(new ClaimQuestEvent(questId));
        }

        private string FormatCondition(ConditionData condition)
        {
            switch (condition.Type)
            {
                case "money_threshold":
                    return $"Earn {condition.Threshold} money";
                case "resource_threshold":
                    return $"Gather {condition.Threshold} {condition.TargetId}";
                case "product_threshold":
                    return $"Craft {condition.Threshold} {condition.TargetId}";
                default:
                    return condition.Type;
            }
        }

        private void RebuildRows()
        {
            if (_list == null || _questState == null)
            {
                return;
            }

            _list.Clear();

            for (int i = 0; i < _questRegistry.Count; i++)
            {
                var definition = _questRegistry[i];

                if (definition != null && _questState.ProgressMap.ContainsKey(definition.Id))
                {
                    _list.Add(BuildQuestCard(definition));
                }
            }
        }
    }
}
