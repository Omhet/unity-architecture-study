## 1. Domain — QuestProgressData and ProgressMap

- [x] 1.1 Create `QuestProgressData` class with `ReactiveProperty<bool>` for `IsClaimable` and `IsCompleted` (default false)
- [x] 1.2 Replace `ObservableList<ActiveQuest> ActiveQuests` in `QuestState` with `Dictionary<string, QuestProgressData> ProgressMap`

## 2. Save Module — QuestSaveModule

- [x] 2.1 Create `QuestSaveModule.cs` in `Assets/_Game/Scripts/Boot/SaveModules/`
- [x] 2.2 Define `QuestSaveData` DTO with `Progress` dictionary property (`Dictionary<string, QuestEntrySaveData>`) and nested `QuestEntrySaveData` with plain bools (`IsCompleted`, `IsClaimable`)
- [x] 2.3 Implement `Key` property returning `"quests"`
- [x] 2.4 Implement `Serialize` — iterate `_questState.ProgressMap`, build scalar entries from `.Value`, store in bundle
- [x] 2.5 Implement `Deserialize` — convert JToken section to `QuestSaveData`, store in bundle
- [x] 2.6 Implement `Validate` — no validation constraints needed (empty implementation)
- [x] 2.7 Implement `Apply` — read `QuestSaveData` from bundle, set `.Value` on existing progress map entries (skip missing quest IDs)

## 3. Flow Handler — Initialize Progress and Wire Evaluators

- [x] 3.1 Update `QuestFlowHandler.On(StartGameEvent)` to iterate registry and ensure each quest ID has a `QuestProgressData` entry in `ProgressMap` (create if missing)
- [x] 3.2 Restore saved state: for quests where save data was applied, the reactive properties already have correct values from `Apply()` — no action needed
- [x] 3.3 Wire evaluators to `ProgressMap[questId].IsClaimable.Value` instead of `ActiveQuest.IsClaimable.Value`
- [x] 3.4 Skip evaluator creation for quests where `IsCompleted.Value` is already true
- [x] 3.5 Update `On(ClaimQuestEvent)` — no change needed (service uses quest ID lookup)

## 4. Service — Update QuestService.Claim to use ProgressMap

- [x] 4.1 Update `Claim(string questId)` to look up progress in `_questState.ProgressMap[questId]` instead of iterating `ActiveQuests`
- [x] 4.2 Get XP reward from registry (`_questRegistry.TryGetById(questId)`) instead of from `ActiveQuest.XpReward`

## 5. View — Update QuestSectionView to use Registry + ProgressMap

- [x] 5.1 Replace `ObservableList<ActiveQuest>` subscription with registry iteration
- [x] 5.2 Build quest cards by iterating `_questRegistry`, reading definition data (ID, XP reward, condition) from each entry
- [x] 5.3 Subscribe to `ProgressMap[def.Id].IsClaimable` and `.IsCompleted` for reactive card updates
- [x] 5.4 Remove collection change observation (`ObserveAdd/Remove/Replace/Reset`) — registry is static

## 6. Cleanup — Remove ActiveQuest

- [x] 6.1 Delete `ActiveQuest.cs`
- [x] 6.2 Remove `ObservableCollections` using directive from `QuestState.cs` (no longer needed)

## 7. DI Registration

- [x] 7.1 Register `QuestSaveModule` as `ISaveModule` in `RootLifetimeScope.cs` alongside other save modules
