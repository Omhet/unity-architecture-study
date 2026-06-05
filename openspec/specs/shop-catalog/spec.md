# Shop Catalog

## Purpose

Defines the shop catalog data structure, registry loading, lookup mechanisms, and cross-reference validation for shop items (recipes and generators).

## Requirements

### Requirement: Shop catalog JSON structure

The shop catalog SHALL be a JSON file with an `items` object containing two arrays: `recipes` and `generators`. Each item in both arrays SHALL have `id` (string), `itemId` (string), and `price` (int) fields.

#### Scenario: Valid recipe shop item

- **WHEN** the catalog contains `{ "id": "shop_recipe_bread", "itemId": "bread_recipe", "price": 100 }` in the `recipes` array
- **THEN** the registry loads the item with all three fields populated

#### Scenario: Valid generator shop item

- **WHEN** the catalog contains `{ "id": "shop_gen_wood", "itemId": "wood_generator", "price": 300 }` in the `generators` array
- **THEN** the registry loads the item with all three fields populated

### Requirement: ShopRegistry loads items from catalog config

The ShopRegistry SHALL provide a `Load(config)` method that populates internal lists from a `ShopCatalogConfig`. Items with null or empty `id` SHALL be skipped.

#### Scenario: Load valid catalog

- **WHEN** `Load()` is called with a catalog containing 2 recipe items and 1 generator item
- **THEN** the registry contains all 3 items accessible by their IDs

#### Scenario: Skip invalid items

- **WHEN** the catalog contains an item with empty `id`
- **THEN** the item is not loaded into the registry

### Requirement: ShopRegistry lookup by ID with type discrimination

The ShopRegistry SHALL provide a `TryGetAny(shopItemId, out shopItemType, out definition)` method that searches both recipe and generator arrays and returns the item type enum alongside the definition.

#### Scenario: Find recipe shop item

- **WHEN** `TryGetAny("shop_recipe_bread")` is called for an existing recipe shop item
- **THEN** it returns true with `ShopItemType.Recipe` and the correct definition

#### Scenario: Find generator shop item

- **WHEN** `TryGetAny("shop_gen_wood")` is called for an existing generator shop item
- **THEN** it returns true with `ShopItemType.Generator` and the correct definition

#### Scenario: Non-existent shop item

- **WHEN** `TryGetAny("nonexistent")` is called with an unknown ID
- **THEN** it returns false with null values

### Requirement: ShopConfigModule validates cross-references

The ShopConfigModule SHALL validate that every recipe shop item's `itemId` exists in the RecipeRegistry and every generator shop item's `itemId` exists in the GeneratorRegistry. Validation errors SHALL be added to the error list with descriptive messages.

#### Scenario: Valid recipe reference

- **WHEN** a recipe shop item references an `itemId` that exists in RecipeCatalogConfig
- **THEN** no validation error is reported for that item

#### Scenario: Invalid generator reference

- **WHEN** a generator shop item references an `itemId` that does not exist in GeneratorCatalogConfig
- **THEN** a validation error is added identifying the unknown reference and the shop item ID

### Requirement: ShopConfigModule validates unique IDs

The ShopConfigModule SHALL validate that all shop item IDs are unique across both recipe and generator arrays.

#### Scenario: Duplicate ID across arrays

- **WHEN** a recipe shop item and a generator shop item share the same `id`
- **THEN** a validation error is reported for the duplicate ID
