# Shop Progression

## Purpose

Defines the progression system that maps player levels to unlocked shop items, including cumulative lookup and reference validation.

## Requirements

### Requirement: ShopProgressionRegistry maps levels to shop item IDs

The ShopProgressionRegistry SHALL load a progression config that maps player levels (int) to lists of shop item IDs (string arrays). The catalog JSON SHALL contain a `progression` array with objects having `level` and `shopItemIds` fields.

#### Scenario: Load progression config

- **WHEN** the config contains entries for levels 1, 2, and 5
- **THEN** the registry stores all three level-to-items mappings

### Requirement: GetUnlockedUpToLevel returns cumulative shop item IDs

The ShopProgressionRegistry SHALL provide a `GetUnlockedUpToLevel(level)` method that returns all shop item IDs from progression entries at or below the given level.

#### Scenario: Level 1 returns only level 1 items

- **WHEN** `GetUnlockedUpToLevel(1)` is called and level 1 has ["shop_recipe_bread"]
- **THEN** the method returns exactly ["shop_recipe_bread"]

#### Scenario: Level 3 returns cumulative items from levels 1-3

- **WHEN** `GetUnlockedUpToLevel(3)` is called with items at levels 1, 2, and 3
- **THEN** the method returns all shop item IDs from those three levels combined

#### Scenario: Level below first entry returns empty

- **WHEN** `GetUnlockedUpToLevel(0)` is called and the lowest progression level is 1
- **THEN** the method returns an empty list

### Requirement: ShopConfigModule validates progression references

The ShopConfigModule SHALL validate that every shop item ID referenced in a progression entry exists in the loaded shop catalog (either recipes or generators arrays).

#### Scenario: Valid progression reference

- **WHEN** a progression entry references "shop_recipe_bread" which exists in the recipes array
- **THEN** no validation error is reported

#### Scenario: Invalid progression reference

- **WHEN** a progression entry references "shop_unknown_item" which does not exist in any shop items array
- **THEN** a validation error is added identifying the unknown shop item ID and the level
