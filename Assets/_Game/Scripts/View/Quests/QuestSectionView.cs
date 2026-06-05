namespace App.Hud.View
{
    using ObservableCollections;
    using App.Flow.Events;
    using App.Quests.Core;
    using App.View;
    using R3;
    using System;
    using UnityEngine.UIElements;
    using VitalRouter;

    public class QuestSectionView : GameplaySectionViewBase
    {
        private readonly QuestState _questState;
        private readonly QuestRegistry _questRegistry;
        private readonly ICommandPublisher _publisher;
        private VisualElement _list;
        private IDisposable _activeQuestsSubscription;

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
            _activeQuestsSubscription?.Dispose();

            if (_questState == null)
            {
                return;
            }

            var updates = Observable.Merge(
                _questState.ActiveQuests.ObserveAdd().Select(_ => Unit.Default),
                _questState.ActiveQuests.ObserveRemove().Select(_ => Unit.Default),
                _questState.ActiveQuests.ObserveReplace().Select(_ => Unit.Default),
                _questState.ActiveQuests.ObserveReset().Select(_ => Unit.Default));

            _activeQuestsSubscription = Observable.Return(Unit.Default)
                .Concat(updates)
                .Subscribe(_ => RebuildRows());
        }

        protected override void Unbind()
        {
            _activeQuestsSubscription?.Dispose();
            _activeQuestsSubscription = null;
        }

        private VisualElement BuildQuestCard(ActiveQuest quest)
        {
            var row = new VisualElement();
            row.AddToClassList("quest-card");

            var title = new Label(quest.Id);
            title.AddToClassList("quest-title");
            row.Add(title);

            // Show condition description from registry
            if (_questRegistry.TryGetById(quest.Id, out var definition) && definition.ConditionData != null)
            {
                var conditionLabel = new Label(FormatCondition(definition.ConditionData));
                conditionLabel.AddToClassList("quest-condition");
                row.Add(conditionLabel);
            }

            var rewardLabel = new Label($"XP: +{quest.XpReward}");
            rewardLabel.AddToClassList("quest-reward");
            row.Add(rewardLabel);

            // Subscribe to IsClaimable and IsCompleted for reactive UI updates
            var claimableSub = quest.IsClaimable.Subscribe(_ => UpdateCardState(row, quest));
            var completedSub = quest.IsCompleted.Subscribe(_ => UpdateCardState(row, quest));
            TrackDisposable(claimableSub);
            TrackDisposable(completedSub);

            // Initial state
            UpdateCardState(row, quest);

            return row;
        }

        private void UpdateCardState(VisualElement card, ActiveQuest quest)
        {
            // Remove old action elements (button or checkmark)
            var oldAction = card.Q<VisualElement>("quest-action");
            if (oldAction != null)
            {
                oldAction.RemoveFromHierarchy();
            }

            if (quest.IsCompleted.Value)
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
                var claimButton = new Button(() => HandleClaimClicked(quest.Id))
                {
                    text = "Claim"
                };
                claimButton.AddToClassList("quest-button");
                claimButton.name = "quest-action";
                claimButton.SetEnabled(quest.IsClaimable.Value);
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

            for (int i = 0; i < _questState.ActiveQuests.Count; i++)
            {
                var quest = _questState.ActiveQuests[i];

                if (quest != null)
                {
                    _list.Add(BuildQuestCard(quest));
                }
            }
        }
    }
}
