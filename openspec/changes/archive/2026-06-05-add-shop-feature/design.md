## Context

The game currently has recipes and generators that are auto-granted on load (first item in each catalog). There is no mechanism for players to acquire new items through purchase. The HUD already has a "shop" placeholder section defined in `HudSectionFactory`.

Existing architecture follows a clear layered pattern:

- **Core**: Registry (catalog lookup), State (observable player data), Service (business logic)
- **Flow**: Events (ICommand structs), Handlers (route events to services)
- **View**: Section views extending `GameplaySectionViewBase`, registered via `HudSectionFactory`
- **Boot**: ConfigModules implementing `IConfigModule` (Deserialize → Validate → Hydrate lifecycle)

The shop must fit this pattern without introducing new architectural concepts.

## Goals / Non-Goals

**Goals:**

- Players can browse available recipes and generators in a shop section view
- Players can purchase items with in-game currency (EconomyService)
- Purchased items are granted as ownership in existing RecipeState/GeneratorState
- Shop availability is driven by player level via ShopProgressionRegistry
- Infinite supply — purchased items remain on the shop shelf for other purchases
- Clean separation: shop availability is independent of player inventory

**Non-Goals:**

- Player progression system (ShopProgressionRegistry exposes `RefreshAvailability(level)` for future integration)
- Purchase confirmation dialogs or "not enough money" feedback in view
- Consumable items, limited stock, or dynamic pricing
- Multiple currencies — single EconomyService Balance is used

## Decisions

### 1. Catalog structure: split arrays by item type

Shop catalog JSON uses separate `recipes` and `generators` arrays rather than a flat list with `itemType` discriminator. Each array contains items of identical shape (`id`, `itemId`, `price`). This avoids runtime type checking and makes validation straightforward (recipe shop items validate against RecipeRegistry, generator items against GeneratorRegistry).

**Alternatives considered:**

- Flat array with `itemType` field — simpler JSON but requires runtime discrimination and more complex validation logic
- Separate config files per type — overkill for two types, adds unnecessary manifest entries

### 2. ShopProgressionRegistry as part of shop config

The level-to-shop-items map lives in the same JSON file as the shop catalog (under a `progression` key) and is loaded by `ShopConfigModule`. This keeps all shop-related config in one place. When progression becomes a real system, this can be extracted to its own module.

**Alternatives considered:**

- Separate config file — cleaner boundary but adds overhead for what's currently a simple lookup table
- Inline `unlockLevel` on each shop item — couples items to progression concept; doesn't support multiple items unlocking at same level as naturally

### 3. No purchased tracking in ShopState

Purchased items are tracked exclusively by RecipeState and GeneratorState (existing ownership systems). The shop does not maintain its own "purchased" list. `RefreshAvailability` only considers player level — it never filters by ownership. This means the shop shelf shows all unlocked items regardless of whether the player already owns them.

**Alternatives considered:**

- Shop tracks purchased IDs — duplicates ownership data, creates sync issues between systems
- Filter out owned items from availability — couples shop to inventory concept; "infinite supply" implies items should stay visible

### 4. ShopService as orchestrator with many dependencies

ShopService depends on ShopRegistry, ShopState, ShopProgressionRegistry, EconomyService, RecipeState, and GeneratorState. This is honest orchestration — the service coordinates between domains without duplicating logic. Each dependency has a single clear responsibility in the buy flow.

**Alternatives considered:**

- Split into two services (availability vs purchase) — premature; both operations are tightly coupled to the same domain concept
- Use events for cross-domain communication — adds indirection for what's currently a simple synchronous operation

### 5. TryGetAny returning type alongside definition

`ShopRegistry.TryGetAny(id)` returns both the `ShopItemDefinition` and an enum indicating whether it's a recipe or generator shop item. This avoids searching two arrays at call sites and makes the buy flow explicit about which ownership state to update.

## Risks / Trade-offs

- **ShopService dependency count** → Six dependencies is high but each has a single clear role. If this becomes unwieldy, we can extract an `IPurchaseTarget` abstraction that encapsulates RecipeState/GeneratorState differences.
- **Progression integration is deferred** → `RefreshAvailability(level)` is a public method with no caller yet. This is intentional — the API surface is ready for when progression arrives.
- **No purchase feedback in view** → If `TrySpend` fails, the buy event fires but nothing happens visibly. The view could subscribe to EconomyState changes and show a toast, but that's out of scope.
