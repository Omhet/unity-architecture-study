## Why

The game needs a shop system where players can purchase recipes and generators with in-game currency. The shop should support gradual item unlocks as player progression is implemented in the future.

## What Changes

- **New Shop domain** — Registry, State, Service, ConfigModule for managing shop items (recipes and generators available for purchase)
- **New ShopProgressionRegistry** — Maps player levels to lists of shop item IDs that become available at each level
- **New ShopSectionView** — UI section in the HUD displaying available shop items with prices and buy buttons
- **New BuyShopItemEvent & ShopFlowHandler** — Flow layer event/handler for purchase actions
- **Shop integration with existing systems** — Shop purchases spend currency via EconomyService and grant ownership via RecipeState / GeneratorState
- **Placeholder "shop" section replaced** — HudSectionFactory currently returns a PlaceholderSectionView for "shop"; will return ShopSectionView

## Capabilities

### New Capabilities

- `shop-catalog`: Shop registry loads shop item definitions (recipes and generators) from JSON catalog config, with cross-reference validation against RecipeRegistry and GeneratorRegistry
- `shop-state`: Shop state tracks currently available shop items; availability is derived from player level via ShopProgressionRegistry, independent of player inventory
- `shop-purchase`: Shop service orchestrates purchases — validates item exists, spends currency via EconomyService, grants ownership to RecipeState/GeneratorState
- `shop-view`: Shop section view renders available items with prices and buy buttons, subscribes to ShopState for reactive updates
- `shop-progression`: ShopProgressionRegistry maps player levels to shop item IDs; service exposes RefreshAvailability(level) for external progression systems to call

### Modified Capabilities

- None

## Impact

- **New files**: ~15 new files in Core (Shop), Flow (Events/Handlers), View (ShopSectionView), Boot (ConfigModule)
- **Existing files modified**: `HudSectionFactory.cs` (replace shop placeholder), `GameLifetimeScope.cs` (register ShopService, ShopFlowHandler), `_game_manifest.json` (add shop catalog entry)
- **Dependencies**: ShopService depends on ShopRegistry, ShopState, ShopProgressionRegistry, EconomyService, RecipeState, GeneratorState. No circular dependencies introduced.
