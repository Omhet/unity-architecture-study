using VitalRouter;

namespace App.Flow.Events
{
    public readonly struct LoadGameEvent : ICommand
    {
    }

    public readonly struct StartGameEvent : ICommand
    {
    }

    public readonly struct GenerateFromGeneratorEvent : ICommand
    {
        public readonly string GeneratorId;

        public GenerateFromGeneratorEvent(string generatorId)
        {
            GeneratorId = generatorId;
        }
    }

    public readonly struct CraftRecipeEvent : ICommand
    {
        public readonly string RecipeId;

        public CraftRecipeEvent(string recipeId)
        {
            RecipeId = recipeId;
        }
    }

    public readonly struct CompleteOrderEvent : ICommand
    {
        public readonly string OrderId;

        public CompleteOrderEvent(string orderId)
        {
            OrderId = orderId;
        }
    }

    public readonly struct BuyShopItemEvent : ICommand
    {
        public readonly string ShopItemId;

        public BuyShopItemEvent(string shopItemId)
        {
            ShopItemId = shopItemId;
        }
    }

    public readonly struct ClaimQuestEvent : ICommand
    {
        public readonly string QuestId;

        public ClaimQuestEvent(string questId)
        {
            QuestId = questId;
        }
    }

    public readonly struct PurchaseTalentEvent : ICommand
    {
        public readonly string TalentId;

        public PurchaseTalentEvent(string talentId)
        {
            TalentId = talentId;
        }
    }

    public readonly struct ExitToMenuEvent : ICommand
    {
    }

    public readonly struct ManualSaveEvent : ICommand
    {
    }

    public readonly struct SelectSlotEvent : ICommand
    {
        public readonly int SlotIndex;

        public SelectSlotEvent(int slotIndex)
        {
            SlotIndex = slotIndex;
        }
    }

    public readonly struct DeleteSlotEvent : ICommand
    {
        public readonly int SlotIndex;

        public DeleteSlotEvent(int slotIndex)
        {
            SlotIndex = slotIndex;
        }
    }
}
