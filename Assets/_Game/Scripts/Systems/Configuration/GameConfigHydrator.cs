namespace App.Systems.Configuration
{
    using System;
    using App.Economy.Core;
    using App.GameConfig.Core;
    using App.Generators.Core;
    using App.Orders.Core;
    using App.Products.Core;
    using App.Progression.Core;
    using App.Quests.Core;
    using App.Resources.Core;
    using App.Shop.Core;
    using App.Talents.Core;
    using App.Unlocks.Core;

    public class GameConfigHydrator
    {
        private readonly EconomyModel _economyModel;
        private readonly GeneratorModel _generatorModel;
        private readonly ResourceModel _resourceModel;
        private readonly ProductInventoryModel _productInventoryModel;
        private readonly ProgressionModel _progressionModel;
        private readonly OrderModel _orderModel;
        private readonly QuestModel _questModel;
        private readonly TalentModel _talentModel;
        private readonly UnlockModel _unlockModel;
        private readonly ShopModel _shopModel;

        public GameConfigHydrator(
            EconomyModel economyModel,
            GeneratorModel generatorModel,
            ResourceModel resourceModel,
            ProductInventoryModel productInventoryModel,
            ProgressionModel progressionModel,
            OrderModel orderModel,
            QuestModel questModel,
            TalentModel talentModel,
            UnlockModel unlockModel,
            ShopModel shopModel)
        {
            _economyModel = economyModel;
            _generatorModel = generatorModel;
            _resourceModel = resourceModel;
            _productInventoryModel = productInventoryModel;
            _progressionModel = progressionModel;
            _orderModel = orderModel;
            _questModel = questModel;
            _talentModel = talentModel;
            _unlockModel = unlockModel;
            _shopModel = shopModel;
        }

        public void Hydrate(GameCatalogBundle bundle)
        {
            HydrateProgression(bundle.Progression);
            HydrateEconomy(bundle.Economy);
            HydrateGenerators(bundle.Generators);
            HydrateResources(bundle.Resources);
            HydrateProducts();
            HydrateOrders();
            HydrateQuests();
            HydrateTalents();
            HydrateShop(bundle.Shop);
        }

        private void HydrateProgression(ProgressionCatalogConfig config)
        {
            if (config == null)
            {
                return;
            }

            _progressionModel.Level.Value = config.StartingLevel;
            _progressionModel.Experience.Value = config.StartingExperience;
            _progressionModel.TalentPoints.Value = config.StartingTalentPoints;
        }

        private void HydrateEconomy(EconomyCatalogConfig config)
        {
            if (config == null)
            {
                return;
            }

            _economyModel.Balance.Value = config.StartingMoney;
        }

        private void HydrateResources(ResourceCatalogConfig config)
        {
            _resourceModel.Clear();
            if (config?.Resources == null)
            {
                return;
            }

            foreach (var resource in config.Resources)
            {
                if (resource == null || string.IsNullOrWhiteSpace(resource.Id))
                {
                    continue;
                }

                _resourceModel.SetAmount(resource.Id, resource.StartingAmount);
            }
        }

        private void HydrateGenerators(GeneratorCatalogConfig config)
        {
            _generatorModel.Generators.Clear();
            if (config?.Generators == null)
            {
                return;
            }

            foreach (var generator in config.Generators)
            {
                if (generator == null || string.IsNullOrWhiteSpace(generator.Id))
                {
                    continue;
                }

                _generatorModel.Generators.Add(new GeneratorModel.GeneratorState
                {
                    Id = generator.Id,
                    DisplayName = generator.DisplayName,
                    ResourceId = generator.ResourceId,
                    AmountPerClick = generator.AmountPerClick
                });
            }

        }

        private void HydrateProducts()
        {
            _productInventoryModel.Products.Clear();
        }

        private void HydrateOrders()
        {
            _orderModel.ActiveOrderIds.Clear();
        }

        private void HydrateQuests()
        {
            _questModel.ProgressByQuestId.Clear();
            _questModel.CompletedByQuestId.Clear();
        }

        private void HydrateTalents()
        {
            _talentModel.UnlockedTalents.Clear();
            _unlockModel.UnlockStates.Clear();
        }

        private void HydrateShop(ShopConfig config)
        {
            _shopModel.Items.Clear();
            if (config?.AvailableItems == null)
            {
                return;
            }

            foreach (var entry in config.AvailableItems)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.ItemId))
                {
                    continue;
                }

                entry.IsVisible = entry.InitialVisibility;
                entry.IsBuyable = entry.InitialBuyability;
                _shopModel.Items.Add(entry);
            }
        }
    }
}
