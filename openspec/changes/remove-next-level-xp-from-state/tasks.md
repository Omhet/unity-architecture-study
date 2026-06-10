## 1. Remove NextLevelXp from ProgressionState

- [x] 1.1 Remove `NextLevelXp` reactive property from `ProgressionState.cs`

## 2. Update ProgressionService

- [x] 2.1 Remove all `_progressionState.NextLevelXp.Value = ...` assignments in `AddXp()` method
- [x] 2.2 Verify the level-up loop still works correctly (it already reads threshold from registry, just no longer writes it back)

## 3. Update ProgressionSaveModule

- [x] 3.1 Remove `NextLevelXp` property from `ProgressionSaveData` struct
- [x] 3.2 Remove `NextLevelXp` assignment in `Serialize()` method
- [x] 3.3 Remove `NextLevelXp` restoration in `Apply()` method

## 4. Clean up ProgressionConfigModule

- [x] 4.1 Remove the TODO comment and commented-out code in `Hydrate()` method

## 5. Update HudShellView

- [x] 5.1 Inject `ProgressionRegistry` into `Construct()` method
- [x] 5.2 Replace `_nextLevelSubscription` with registry-based threshold lookup — subscribe to both `Level` and `Xp`, and on each callback call `_progressionRegistry.GetNextLevelXpForLevel(_progressionState.Level.Value)` to derive the threshold for `UpdateXp()`
