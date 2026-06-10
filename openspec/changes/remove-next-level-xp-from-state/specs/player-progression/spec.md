## REMOVED Requirements

### Requirement: Track NextLevelXp in Reactive State

**Reason**: `NextLevelXp` is derived data — fully determined by the player's current level and the progression catalog. Storing it as state creates redundancy, wastes save file space, and risks drift when config values change. The registry already provides `GetNextLevelXpForLevel(level)` as the single source of truth.

**Migration**: Consumers that previously read `_progressionState.NextLevelXp.Value` now call `_progressionRegistry.GetNextLevelXpForLevel(_progressionState.Level.Value)`. The `ProgressionService` no longer writes to this property, and the HUD queries the registry directly on demand.
