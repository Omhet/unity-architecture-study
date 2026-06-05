## Why

Players currently have no way to customize their progression beyond leveling up. A talent system gives players meaningful choices about how to scale their gameplay — boosting generation, crafting, or order rewards — adding strategic depth and replayability.

## What Changes

- **New Talents subsystem** — Core layer with `TalentState`, `TalentRegistry`, `TalentService`, config-driven talent definitions (id, cost, increasePerPoint, maxPoints)
- **Talent multipliers in existing services** — `GeneratorService`, `CraftService`, `OrderService` each apply a talent multiplier to their output amounts
- **Level-up grants talent points** — Each level up gives the player 1 spendable talent point via `LevelUpFlowHandler`
- **Talent purchase flow** — New event + handler for spending points on talents from the UI
- **Talent section view** — HUD panel showing available points, talent list with current investment and buy buttons

## Capabilities

### New Capabilities

- `talent-core`: Talent state management (available points, per-talent points spent), registry of config-driven talent definitions, purchase logic with cost/max validation
- `talent-multipliers`: Apply talent multipliers in GeneratorService, CraftService, and OrderService based on points invested
- `talent-flow`: Level-up grants talent points, purchase event/handler bridges UI to TalentService
- `talent-view`: HUD section displaying point balance, talent list with current investment levels and buy buttons

### Modified Capabilities

- `player-progression`: Level-up now has an additional side-effect of granting 1 talent point

## Impact

- **Core**: New `Core/Talents/` folder with state, registry, service, catalog config, entry model
- **Services**: `GeneratorService`, `CraftService`, `OrderService` each gain a dependency on `TalentService` for multiplier lookups
- **Flow**: New event (`PurchaseTalentEvent`), new handler (`TalentFlowHandler`), modify `LevelUpFlowHandler` to grant points
- **View**: New `View/Talents/TalentSectionView.cs` HUD section
- **Config**: New `TalentConfigModule`, JSON config file with talent definitions
