## Context

The current quest model has `ActiveQuest` instances that duplicate static config data (ID, XP reward) already present in `QuestRegistry`. The only runtime state is `IsClaimable` and `IsCompleted`, but it's buried inside objects that also carry redundant definition data. Additionally, the save system loads before `StartGameEvent` fires, so there are no `ActiveQuest` objects to restore onto when a save module runs.

**Current model (duplicated):**

```
┌─────────────────────┐         ┌──────────────────────────┐
│   QuestRegistry      │         │   QuestState             │
│  (config, static)    │         │  (runtime state)          │
│                      │         │                          │
│  quest_001           │         │  ActiveQuest:            │
│    Id: "quest_001"   │◀─dup──▶│    Id: "quest_001"       │
│    XpReward: 50      │◀─dup──▶│    XpReward: 50           │
│    ConditionData: ... │         │    IsClaimable: false    │
│                      │         │    IsCompleted: false     │
└─────────────────────┘         └──────────────────────────┘

PROPOSED (single source of truth):
┌─────────────────────┐         ┌──────────────────────────┐
│   QuestRegistry      │         │   QuestState             │
│  (config, static)    │         │  (only what changes)     │
│                      │         │                          │
│  quest_001           │         │  ProgressMap:            │
│    Id: "quest_001"   │         │    "quest_001" → {       │
│    XpReward: 50      │         │      IsClaimable,        │
│    ConditionData: ... │         │      IsCompleted }       │
└─────────────────────┘         └──────────────────────────┘
```

**Current boot sequence:**

```
LoadGameEvent → Config hydration → Save load (ISaveModule.Apply) → Scene load → StartGameEvent
                                                                         └─ QuestFlowHandler creates ActiveQuests here
```

## Goals / Non-Goals

**Goals:**

- Eliminate `ActiveQuest` — registry is the single source of truth for quest definitions
- `QuestState` tracks only player progress via `ProgressMap` with reactive properties
- Persist quest progress (`IsCompleted`, `IsClaimable`) per quest ID across sessions
- Restore saved state during boot so quests reflect player's actual progress on game start
- Keep the save module simple (plain dictionary of scalars) following existing patterns
- Skip evaluator wiring for already-completed quests (no point observing conditions that won't change)
- UI iterates registry for definitions, reads progress from state map for card appearance

**Non-Goals:**

- Quest save migrations (first version, no legacy data to migrate)
- Conditional saving (all quest progress is always saved)
- Quest unlock/chain state (quests are all active from the start)

## Decisions

### Decision 1: Registry is Source of Truth, State Tracks Only Progress

**Choice:** Remove `ActiveQuest` entirely. `QuestRegistry` owns definition data (ID, XP reward, condition). `QuestState.ProgressMap` owns only runtime progress (`IsClaimable`, `IsCompleted`) as reactive properties keyed by quest ID.

```
Save Module Apply()                    StartGameEvent Handler
─────────────────                      ──────────────────────
Restores ProgressMap:                  Iterates registry:
  "quest_001" → { completed }          ├─ Ensure ProgressMap has entry for each quest
  "quest_002" → {}                     ├─ Restore saved state into reactive properties
                                       ├─ Wire evaluator (skip if completed)
```

**Rationale:**

- No data duplication — ID, XP reward, condition live in one place (registry)
- State is minimal — only tracks what actually changes at runtime
- UI composes from two sources: registry (what exists) + state (player progress)
- New quests from content updates appear naturally (registry iteration creates missing map entries)
- Removed quests are ignored naturally (registry iteration is the source of truth)

**Alternatives considered:**

- Keep `ActiveQuest` with save reconciliation → still duplicates config data, just adds complexity
- Save module creates progress entries directly → couples persistence to reactive property construction

### Decision 2: QuestProgressData Holds Reactive Properties

**Choice:** `QuestProgressData` contains `ReactiveProperty<bool>` for both flags. The UI subscribes to these properties directly from the map values.

```csharp
public class QuestProgressData
{
    public ReactiveProperty<bool> IsClaimable { get; } = new(false);
    public ReactiveProperty<bool> IsCompleted { get; } = new(false);
}
```

**Rationale:**

- UI subscribes to `progress.IsCompleted.Subscribe(...)` for reactive card updates — same pattern as current `ActiveQuest` subscriptions
- Observable dictionary is NOT needed — the dictionary notifies on structural changes (add/remove keys), but we need reactivity from property mutations inside values. The properties themselves provide that.
- Save module serializes `.Value` scalars, deserializes into plain DTOs, then applies to reactive properties

### Decision 3: UI Iterates Registry + Reads Progress Map

**Choice:** `QuestSectionView.Bind()` iterates `_questRegistry`, looks up each quest's progress in `_questState.ProgressMap[def.Id]`, and subscribes to the reactive properties.

```
UI renders:  Registry[i] → ProgressMap[Registry[i].Id]
              definition       runtime state
```

**Rationale:**

- No need for `ObservableList` collection events — registry is static, progress changes are observed through reactive properties
- Card rebuilds on claim/completion via property subscriptions, not collection change events
- Simpler subscription model: one subscription per quest per property, keyed by stable quest ID

### Decision 4: Skip Evaluator Wiring for Completed Quests

**Choice:** In `QuestFlowHandler.On(StartGameEvent)`, if a quest's progress shows `IsCompleted = true`, skip creating and subscribing the evaluator.

**Rationale:**

- Completed quests can't be claimed again — no need to observe their condition
- Saves memory and CPU by not keeping dead subscriptions alive
- The `_evaluatorSubscriptions` dictionary won't have an entry, so `ClaimQuestEvent` handler's disposal logic is a harmless no-op (already guarded by `TryGetValue`)

### Decision 5: Save Data Shape

**Choice:** Plain scalar DTOs for serialization — the reactive properties are runtime-only.

```json
{
  "quests": {
    "progress": {
      "quest_001": { "isCompleted": true, "isClaimable": false },
      "quest_002": { "isCompleted": false, "isClaimable": true }
    }
  }
}
```

**Rationale:**

- `IsClaimable` is saved so if a condition was already met before quitting, it's restored correctly
- Minimal footprint — two booleans per quest
- Save module reads `.Value` from reactive properties for serialization, writes to `.Value` on apply

## Risks / Trade-offs

- **Breaking change** — `ActiveQuest` is removed, `QuestState.ActiveQuests` becomes `ProgressMap`. Any code referencing these types needs updating. Mitigation: this is a study project with controlled surface area.
- **Registry/Save mismatch** — If a quest is removed from the registry but exists in save data, it's silently ignored. This is acceptable behavior; orphaned save entries are harmless.
