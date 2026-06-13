## Why

Quest state is lost between sessions — there's no persistence for which quests are claimed or completed. Additionally, the current model duplicates static config data (quest ID, XP reward) inside `ActiveQuest` instances when that information already exists in `QuestRegistry`. The save system has infrastructure (`ISaveModule`, `SaveLoadSystem`) but no quest module.

## What Changes

- **Remove `ActiveQuest`** — eliminate the duplicated runtime object; registry is the single source of truth for quest definitions
- **Replace with `ProgressMap` in `QuestState`** — `Dictionary<string, QuestProgressData>` where each entry holds reactive properties (`IsClaimable`, `IsCompleted`). State tracks only what changes at runtime.
- **Add `QuestSaveModule`** — new `ISaveModule` that serializes/deserializes progress map scalars to/from save data
- **Update `QuestFlowHandler.On(StartGameEvent)`** — initialize progress entries from registry, restore saved state, wire evaluators directly to progress properties (skip completed quests)
- **Update `QuestSectionView`** — iterate registry for definitions, read progress from state map for card appearance
- **Register `QuestSaveModule` in DI** — add to `RootLifetimeScope` alongside other save modules

## Capabilities

### New Capabilities

- `quest-save`: Quest progress persistence via save module — serializes per-quest completion and claimable state, restores through `ProgressMap`

### Modified Capabilities

- `quest-management`: Remove `ActiveQuest`, replace with `ProgressMap` of reactive properties in `QuestState`. Registry is source of truth for definitions, state tracks only player progress. Completed quests skip evaluator wiring.

## Impact

**Code Changes:**

- `QuestSaveModule.cs` — new file in `Assets/_Game/Scripts/Boot/SaveModules/`
- `QuestProgressData.cs` — new class with reactive properties (`IsClaimable`, `IsCompleted`)
- `QuestState.cs` — **BREAKING**: replace `ObservableList<ActiveQuest> ActiveQuests` with `Dictionary<string, QuestProgressData> ProgressMap`
- `ActiveQuest.cs` — **REMOVED**
- `QuestFlowHandler.cs` — update to work with progress map instead of active quest list
- `QuestSectionView.cs` — **BREAKING**: iterate registry + progress map instead of active quest collection
- `RootLifetimeScope.cs` — register `QuestSaveModule` as `ISaveModule`

**Dependencies:**

- Follows existing `ISaveModule` pattern (DTO + bundle, same file structure as `ProgressionSaveModule`)
- No breaking changes to existing save modules or save schema
