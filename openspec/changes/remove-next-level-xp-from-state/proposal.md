## Why

`NextLevelXp` in `ProgressionState` is derived data — it's fully determined by the player's current level and the progression catalog (config). Storing it as state creates redundancy, wastes save file space, and risks drift when config values change. It should be removed from state and always looked up from the registry instead.

## What Changes

- **Remove** `NextLevelXp` reactive property from `ProgressionState`
- **Remove** `NextLevelXp` from `ProgressionSaveData` — no longer serialized to save files
- **Update** `ProgressionService.AddXp()` to stop writing `NextLevelXp` into state (registry already has the answer)
- **Update** `HudShellView` to query `ProgressionRegistry.GetNextLevelXpForLevel()` directly instead of subscribing to a reactive property
- **Remove** TODO comment in `ProgressionConfigModule.Hydrate()` — no initialization needed

## Capabilities

### New Capabilities

<!-- None — this is a refactoring, not a new capability -->

### Modified Capabilities

- `player-progression`: State shape changes — `NextLevelXp` is removed from `ProgressionState`. The progression system now derives next-level thresholds from the registry on demand rather than caching them in state.
- `save-modules`: `ProgressionSaveModule` no longer serializes/deserializes `NextLevelXp`. Save data for progression contains only `level` and `xp`.

## Impact

- **ProgressionState.cs** — removes `NextLevelXp` property
- **ProgressionService.cs** — stops writing to `_progressionState.NextLevelXp`
- **ProgressionSaveModule.cs** — removes `NextLevelXp` from save data struct, serialize, deserialize, and apply logic
- **HudShellView.cs** — injects `ProgressionRegistry`, queries it for threshold instead of subscribing to reactive property
- **ProgressionConfigModule.cs** — removes TODO comment about initializing `NextLevelXp`
- **Existing save files** — will have an extra `nextLevelXp` field that is ignored on load (backward compatible)
