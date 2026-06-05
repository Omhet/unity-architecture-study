## ADDED Requirements

### Requirement: Buy spends currency from EconomyService

The ShopService SHALL provide a `Buy(shopItemId)` method that first looks up the shop item in ShopRegistry, then calls `EconomyService.TrySpend(price)`. If spending fails, the purchase SHALL be aborted and no ownership is granted.

#### Scenario: Successful spend

- **WHEN** `Buy("shop_recipe_bread")` is called with sufficient balance
- **THEN** EconomyService.TrySpend(100) is called and returns true

#### Scenario: Insufficient balance aborts purchase

- **WHEN** `Buy("shop_recipe_bread")` is called with insufficient balance
- **THEN** EconomyService.TrySpend returns false and no ownership is granted to any state

### Requirement: Buy grants recipe ownership for recipe shop items

When purchasing a recipe shop item, the ShopService SHALL add the referenced `itemId` to `RecipeState.PlayerOwnedRecipeIds`.

#### Scenario: Purchase recipe

- **WHEN** `Buy("shop_recipe_bread")` succeeds and the item type is Recipe with itemId "bread_recipe"
- **THEN** "bread_recipe" is added to RecipeState.PlayerOwnedRecipeIds

### Requirement: Buy grants generator ownership for generator shop items

When purchasing a generator shop item, the ShopService SHALL add the referenced `itemId` to `GeneratorState.PlayerOwnedGeneratorIds`.

#### Scenario: Purchase generator

- **WHEN** `Buy("shop_gen_wood")` succeeds and the item type is Generator with itemId "wood_generator"
- **THEN** "wood_generator" is added to GeneratorState.PlayerOwnedGeneratorIds

### Requirement: Buy does not affect shop availability

A successful purchase SHALL NOT remove the shop item from ShopState.AvailableShopItemIds. The item remains available for future purchases (infinite supply).

#### Scenario: Item remains after purchase

- **WHEN** `Buy("shop_recipe_bread")` succeeds
- **THEN** "shop_recipe_bread" is still present in ShopState.AvailableShopItemIds
