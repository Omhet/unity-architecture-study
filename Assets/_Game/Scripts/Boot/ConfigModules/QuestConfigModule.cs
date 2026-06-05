namespace App.Boot.ConfigModules
{
    using System.Collections.Generic;
    using App.Products.Core;
    using App.Quests.Core;
    using App.Resources.Core;
    using App.Systems.Configuration;
    using Newtonsoft.Json;

    public class QuestConfigModule : IConfigModule
    {
        private readonly QuestRegistry _questRegistry;

        public string Key => "quests";

        public QuestConfigModule(QuestRegistry questRegistry)
        {
            _questRegistry = questRegistry;
        }

        public void Deserialize(string json, GameCatalogBundle bundle)
        {
            var config = JsonConvert.DeserializeObject<QuestCatalogConfig>(json);
            bundle.SetConfig(Key, config);
        }

        public void Validate(GameCatalogBundle bundle, List<string> errors)
        {
            var quests = bundle.GetConfig<QuestCatalogConfig>(Key);
            if (quests?.Quests == null)
            {
                errors.Add("Missing quests catalog.");
                return;
            }

            Utility.ConfigValidationHelper.ValidateUniqueIds(quests.Quests, x => x?.Id, "quest", errors);

            var resources = bundle.GetConfig<ResourceCatalogConfig>("resources");
            var products = bundle.GetConfig<ProductCatalogConfig>("products");
            var resourceIds = Utility.ConfigValidationHelper.BuildIdSet(resources?.Resources, x => x?.Id);
            var productIds = Utility.ConfigValidationHelper.BuildIdSet(products?.Products, x => x?.Id);

            foreach (var quest in quests.Quests)
            {
                if (quest == null)
                {
                    continue;
                }

                if (quest.ConditionData == null)
                {
                    errors.Add("Quest is missing condition data: " + quest.Id);
                    continue;
                }

                var conditionType = quest.ConditionData.Type;
                bool typeKnown = conditionType == "money_threshold" || conditionType == "resource_threshold" || conditionType == "product_threshold";
                if (!typeKnown)
                {
                    errors.Add("Quest has unknown condition type: " + conditionType + " (quest: " + quest.Id + ")");
                }

                if (conditionType == "resource_threshold")
                {
                    if (!resourceIds.Contains(quest.ConditionData.TargetId))
                    {
                        errors.Add("Quest references unknown resource: " + quest.ConditionData.TargetId + " (quest: " + quest.Id + ")");
                    }
                }

                if (conditionType == "product_threshold")
                {
                    if (!productIds.Contains(quest.ConditionData.TargetId))
                    {
                        errors.Add("Quest references unknown product: " + quest.ConditionData.TargetId + " (quest: " + quest.Id + ")");
                    }
                }

                if (quest.ConditionData.Threshold <= 0)
                {
                    errors.Add("Quest has non-positive threshold: " + quest.ConditionData.Threshold + " (quest: " + quest.Id + ")");
                }

                if (quest.XpReward <= 0)
                {
                    errors.Add("Quest has non-positive XP reward: " + quest.XpReward + " (quest: " + quest.Id + ")");
                }
            }
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            var config = bundle.GetConfig<QuestCatalogConfig>(Key);
            _questRegistry.Load(config);
        }
    }
}
