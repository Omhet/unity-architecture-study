namespace App.GameConfig.Core
{
    using System;

    [Serializable]
    public class GameConfigManifest
    {
        public int SchemaVersion;
        public ConfigCatalogEntry[] Catalogs;
    }

    [Serializable]
    public class ConfigCatalogEntry
    {
        public string Key;
        public string Address;
        public int SchemaVersion;
    }

    [Serializable]
    public class ResourceCatalogConfig
    {
        public ResourceDefinition[] Resources;
    }

    [Serializable]
    public class ResourceDefinition
    {
        public string Id;
        public string DisplayName;
        public int StartingAmount;
    }

    [Serializable]
    public class ProductCatalogConfig
    {
        public ProductDefinition[] Products;
    }

    [Serializable]
    public class ProductDefinition
    {
        public string Id;
        public string DisplayName;
    }

    [Serializable]
    public class GeneratorCatalogConfig
    {
        public GeneratorDefinition[] Generators;
    }

    [Serializable]
    public class GeneratorDefinition
    {
        public string Id;
        public string DisplayName;
        public string ResourceId;
        public int AmountPerClick;
    }

    [Serializable]
    public class RecipeCatalogConfig
    {
        public RecipeDefinition[] Recipes;
    }

    [Serializable]
    public class RecipeDefinition
    {
        public string Id;
        public string DisplayName;
        public IngredientRequirement[] ResourceInputs;
        public ProductOutput ProductOutput;
    }

    [Serializable]
    public class IngredientRequirement
    {
        public string ResourceId;
        public int Amount;
    }

    [Serializable]
    public class ProductOutput
    {
        public string ProductId;
        public int Amount;
    }

    [Serializable]
    public class OrderCatalogConfig
    {
        public OrderDefinition[] Orders;
    }

    [Serializable]
    public class OrderDefinition
    {
        public string Id;
        public string CustomerName;
        public int Payout;
        public ProductRequirement[] ProductRequirements;
    }

    [Serializable]
    public class ProductRequirement
    {
        public string ProductId;
        public int Amount;
    }

    [Serializable]
    public class QuestCatalogConfig
    {
        public QuestDefinition[] Quests;
    }

    [Serializable]
    public class QuestDefinition
    {
        public string Id;
        public string DisplayName;
        public string CounterType;
        public string CounterTargetId;
        public int RequiredAmount;
        public int RewardExperience;
    }

    [Serializable]
    public class ProgressionCatalogConfig
    {
        public int StartingLevel;
        public int StartingExperience;
        public int StartingTalentPoints;
        public LevelDefinition[] Levels;
    }

    [Serializable]
    public class EconomyCatalogConfig
    {
        public int StartingMoney;
    }

    [Serializable]
    public class LevelDefinition
    {
        public int Level;
        public int RequiredExperience;
        public int TalentPointsAwarded;
    }

    [Serializable]
    public class TalentCatalogConfig
    {
        public TalentDefinition[] Talents;
    }

    [Serializable]
    public class TalentDefinition
    {
        public string Id;
        public string DisplayName;
        public string ModifierType;
        public float ModifierValue;
        public int RequiredLevel;
    }

    public class GameCatalogBundle
    {
        public GameConfigManifest Manifest;
        public ResourceCatalogConfig Resources;
        public ProductCatalogConfig Products;
        public GeneratorCatalogConfig Generators;
        public RecipeCatalogConfig Recipes;
        public OrderCatalogConfig Orders;
        public App.Shop.Core.ShopConfig Shop;
        public QuestCatalogConfig Quests;
        public ProgressionCatalogConfig Progression;
        public EconomyCatalogConfig Economy;
        public TalentCatalogConfig Talents;
    }
}
