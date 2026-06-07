# Economy & Resources Save Modules — Design

## Context

The save system has a complete orchestration layer (`SaveLoadSystem`, `SlotManager`, `FileSystemSaveStorage`) with an `ISaveModule` interface, but no concrete implementations. `IEnumerable<ISaveModule>` resolves to an empty collection, so the load pipeline skips deserialization and the save pipeline collects nothing.

The boot sequence in `SceneFlowHandler.On(PlayGameEvent)` is:

1. Load game scene
2. Hydrate all config (registries + default state values)
3. Call `SaveLoadSystem.LoadSlotAsync()` — currently a no-op per-domain

This means save modules deserialize **after** config hydration has already initialized domain state with defaults.

## Goals / Non-Goals

**Goals:**

- Implement `EconomySaveModule` and `ResourceSaveModule` as concrete `ISaveModule` implementations
- Register both in `RootLifetimeScope` so the DI container discovers them
- Validate the end-to-end save/load pipeline works for these two domains
- Establish a reusable pattern for the remaining 8 domain modules

**Non-Goals:**

- Implementing save modules for other domains (Generators, Products, Recipes, Orders, Shop, Progression, Quests, Talents)
- Adding new migration versions or migration logic
- Modifying `ISaveModule` interface or `SaveLoadSystem` orchestration
- UI integration for save/load triggers

## Decisions

### 1. Save modules live in `Boot/SaveModules/`, not co-located with domains

**Decision:** Place both modules under `Assets/_Game/Scripts/Boot/SaveModules/`.

**Rationale:** Semantically, save modules are part of game booting — they wire the persistence layer during initialization, just like config modules live in `Boot/ConfigModules/`. This keeps all "boot wiring" in one folder. The tradeoff is that domain folders don't contain their own serialization logic, but that's acceptable since save modules are plumbing, not business logic.

### 2. Anonymous objects for economy serialization, Dictionary for resources

**Decision:** Use `new { balance = _state.Balance.Value }` for Economy and `new Dictionary<string, int>(_state.Balances)` for Resources.

**Rationale:** Anonymous objects avoid dedicated DTO boilerplate for simple shapes. Newtonsoft.Json serializes them cleanly. For Resources, the state is already a dictionary-like structure (`ObservableDictionary<string, int>`), so copying to a plain `Dictionary` is natural and requires no transformation.

### 3. Merge strategy for ResourceSaveModule deserialization

**Decision:** Iterate over saved key-value pairs and call `_state.SetAmount()` for each, without clearing the state first.

**Rationale:** Config hydration runs before save load and already populates all resource keys at 0. Merging means new resources (added after a save was written) keep their default value of 0, while saved values overwrite known keys. No special handling needed for orphaned or missing keys.

### 4. Save modules depend only on State, not Registry

**Decision:** `EconomySaveModule` depends on `EconomyState`. `ResourceSaveModule` depends on `ResourceState`. Neither takes a registry dependency.

**Rationale:** Keep save modules thin and decoupled. They're responsible for translating between JSON shape and runtime state — they don't need to validate against registries. Orphaned keys in save data are harmless (nothing reads them), and missing keys are handled by the merge strategy retaining defaults.

### 5. Validation: values must be >= 0

**Decision:** Both modules validate that all numeric values are non-negative.

**Rationale:** Negative balance or negative resource amounts don't make semantic sense in this game. This is a data integrity check, not business logic. Migrations handle schema evolution; validation catches corruption or manual edits to save files.

## Risks / Trade-offs

| Risk                                                                                      | Mitigation                                                                                                                                                       |
| ----------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Save file contains keys for resources no longer in config                                 | Harmless — `SetAmount()` will set values on state entries that nothing reads. Could add a future cleanup migration if this becomes a concern.                    |
| Economy balance deserializes to unexpected value (e.g., int overflow from corrupted data) | Validation catches negative values. Extremely large positive values are unlikely in practice and would just give the player lots of currency — not a crash risk. |
| `ObservableDictionary` copy behavior changes in future library update                     | The copy constructor pattern (`new Dictionary(source)`) is standard .NET, not library-specific. Low risk.                                                        |

## Open Questions

- Should we add integration tests that write a save file to disk and verify load restores state? (Out of scope for this change, but worth considering as a follow-up.)
