## Why

The game lacks a structured progression system. Players currently have no long-term goals or rewards for their actions. Introducing a player level and XP system provides a core loop of engagement, where actions result in XP, leading to levels that unlock new content (like shop items).

## What Changes

- Introduce **ProgressionState** to track Level, Current XP, and XP required for the next level.
- Create **ProgressionService** to handle XP addition and level-up logic.
- Implement **ProgressionRegistry** and **ProgressionConfig** to manage level-to-XP requirements via a JSON-backed "table".
- Add **LevelUpFlowHandler** to reactively refresh the shop and handle potential side effects when the player levels up.
- Update **HudShellView** to display level and progression progress in the status bar.

## Capabilities

### New Capabilities

- `player-progression`: Core logic for XP tracking, level-up calculations, and configuration-driven milestones.

### Modified Capabilities

- `shop`: Update requirements to include reactive refresh when the player's level increases.

## Impact

- **App.Core**: New `Progression` module.
- **App.Shop.Core**: `ShopService` and `ShopConfigModule` will be integrated with the new progression system.
- **App.Flow**: New `LevelUpFlowHandler` to bridge Core events to Flow-level side effects.
- **App.View**: `HudShellView` UI updates to include progression indicators.
- **App.Systems.Configuration**: New config module for progression milestones.
