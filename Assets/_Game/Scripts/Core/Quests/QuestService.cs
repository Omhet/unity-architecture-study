namespace App.Quests.Core
{
    using App.Economy.Core;
    using App.Products.Core;
    using App.Progression.Core;
    using App.Quests.Core.Conditions;
    using App.Resources.Core;

    public class QuestService
    {
        private readonly QuestState _questState;
        private readonly ProgressionService _progressionService;
        private readonly EconomyState _economyState;
        private readonly ResourceState _resourceState;
        private readonly ProductState _productState;

        public QuestService(
            QuestState questState,
            ProgressionService progressionService,
            EconomyState economyState,
            ResourceState resourceState,
            ProductState productState)
        {
            _questState = questState;
            _progressionService = progressionService;
            _economyState = economyState;
            _resourceState = resourceState;
            _productState = productState;
        }

        public void Claim(string questId)
        {
            ActiveQuest targetQuest = null;

            for (int i = 0; i < _questState.ActiveQuests.Count; i++)
            {
                var quest = _questState.ActiveQuests[i];
                if (quest != null && quest.Id == questId)
                {
                    targetQuest = quest;
                    break;
                }
            }

            if (targetQuest == null)
            {
                return;
            }

            if (!targetQuest.IsClaimable.Value)
            {
                return;
            }

            if (targetQuest.IsCompleted.Value)
            {
                return;
            }

            _progressionService.AddXp(targetQuest.XpReward);
            targetQuest.IsCompleted.Value = true;
        }

        public IConditionEvaluator CreateEvaluator(ConditionData conditionData)
        {
            if (conditionData == null)
            {
                return null;
            }

            switch (conditionData.Type)
            {
                case "money_threshold":
                    return new MoneyThresholdEvaluator(_economyState, conditionData.Threshold);
                case "resource_threshold":
                    return new ResourceThresholdEvaluator(_resourceState, conditionData.TargetId, conditionData.Threshold);
                case "product_threshold":
                    return new ProductThresholdEvaluator(_productState, conditionData.TargetId, conditionData.Threshold);
                default:
                    throw new System.NotImplementedException("Unknown condition type: " + conditionData.Type);
            }
        }
    }
}
