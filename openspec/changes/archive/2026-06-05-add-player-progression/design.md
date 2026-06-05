## Context

The game currently has no progression system. The project follows a clean architecture pattern where logic is divided into **Core** (pure domain logic), **Flow** (application orchestration/use cases), and **View** (UI). We need to implement a progression system that adheres to these boundaries.

## Goals / Non-Goals

**Goals:**

- Implement a persistent Level/XP state in `App.Core`.
- Use `R3` for reactive updates to level and XP.
- Load level requirements from a JSON configuration via the existing `IConfigModule` system.
- Trigger shop refreshes automatically when the player levels up via a `LevelUpFlowHandler`.
- Display level progress in the HUD Status Bar.

**Non-Goals:**

- No visual "Level Up" popups or animations in this phase (just state/logic/HUD).
- No complex XP decay or multiplayer synchronization.
- No "Level Up Command" in the message bus (subscribing to state directly in Flow is preferred).

## Decisions

### 1. State Structure (App.Core)

**Decision**: Use `ReactiveProperty<int>` for `Level`, `Xp`, and `NextLevelXp`.

- **Rationale**: Follows existing patterns in `EconomyState` and `ResourceState`. Allows easy binding for UI and Flow handlers.
- **Alternatives**: Using standard `C#` events (discarded for consistency with `R3` reactive architecture).

### 2. Configuration Handling (App.Boot)

**Decision**: Create a `ProgressionConfigModule` that implements `IConfigModule`.

- **Rationale**: Plugs into the existing `GameCatalogBundle` system used by Recipes, Products, and Shop.
- **Data Model**: `ProgressionConfig` will hold an array of `LevelEntry { Level, NextLevelXp }`.

### 3. Logic Layering (App.Flow)

**Decision**: Use `LevelUpFlowHandler` to bridge Core to Flow side-effects.

- **Rationale**: Core services shouldn't know about UI or other orchestrators. The Flow handler listens for state changes and coordinates the `ShopService` refresh.
- **Trigger**: `IInitializable.Initialize` subscribes to `ProgressionState.Level` changes.

### 4. UI Integration (App.View)

**Decision**: Extend `HudShellView.BuildStatusBar()` to include Level and XP labels.

- **Rationale**: Minimal invasive change to the existing HUD structure.

## Risks / Trade-offs

- **[Risk] High Frequency Level Ups** → If a player earns massive XP, the logic must handle multiple level-ups in a single `AddXp` call.
  - **Mitigation**: Use a `while` loop in `ProgressionService.AddXp` to process multiple milestones.
- **[Risk] Sync Issues on Load** → `ShopService` must be refreshed immediately after the save game is loaded, not just on level-up.
  - **Mitigation**: `LevelUpFlowHandler` will call `RefreshAvailability` once during `Initialize` using the current state's level.
