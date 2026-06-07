## Context

Currently slot count and active slot state are scattered across three types with no wiring between them:

- `SaveBootstrapOptions` — registered in DI with `SlotCount = 4` and `ActiveSlotIndex = 0`, but **nothing consumes it**. Dead configuration.
- `SlotManager` — has `_slotCount` from a constructor default parameter (always 4, never injected). Delegates load/save to `SaveLoadSystem`.
- `SaveLoadSystem` — owns `_activeSlot` as a field with hardcoded default of 0. Exposes `GetActiveSlot()` / `SetActiveSlot()`.

The flow layer (`SceneFlowHandler`, `SaveOnQuitSystem`) bypasses `SlotManager` and calls `SaveLoadSystem.GetActiveSlot()` directly, creating a leaky abstraction.

## Goals / Non-Goals

**Goals:**

- Single source of truth for slot count (from `SaveBootstrapOptions`) and active slot (mutable state in `SlotManager`)
- Stateless `SaveLoadSystem` — no internal slot tracking, just receives `slotIndex` as a parameter
- Flow layer depends on both `SlotManager` (for state) and `SaveLoadSystem` (for pipeline), orchestrating the call itself

**Non-Goals:**

- Adding new save features (cloud sync, encryption, etc.)
- Changing the save/load pipeline logic (backup, migrate, validate, serialize/deserialize)
- Changing storage backends or slot file format

## Decisions

### Decision 1: `SlotManager` owns both slot count and active slot

**Why**: `SlotManager` is already the "slot data service" — it lists slots, deletes slots, and validates slot indices. Adding `_activeSlot` here consolidates all slot-related state in one place. The flow layer asks `SlotManager` for the current slot, then passes it to whichever system needs it.

**Alternatives considered:**

- Dedicated `SlotContext` class — would be cleaner separation of config vs state but adds another DI registration and indirection for a two-value object. Overkill at this scale.
- Keeping active slot in `SaveLoadSystem` — mixes pipeline concerns with state management, which is the current problem we're solving.

### Decision 2: Flow layer depends on both `SlotManager` and `SaveLoadSystem`

**Why**: The flow layer is the orchestrator — it decides _which_ slot to load/save (from `SlotManager`) and _how_ (by calling `SaveLoadSystem`). This keeps `SlotManager` as a pure state manager without pipeline knowledge, and `SaveLoadSystem` as a pure pipeline without state.

**Alternatives considered:**

- `SlotManager` delegates to `SaveLoadSystem` — would couple the two systems and hide the pipeline behind the state manager. Flow layer loses visibility into what's happening.
- Only depend on `SlotManager` — same coupling problem; `SlotManager` becomes a god object that knows about storage, pipeline, and state.

### Decision 3: Remove `ActiveSlotIndex` from `SaveBootstrapOptions`

**Why**: The active slot is runtime state (changes when the player switches slots), not boot configuration. `SlotCount` stays because it's immutable after boot — true configuration.

## Risks / Trade-offs

- [Risk] Flow layer now has two dependencies instead of one → This is a feature, not a bug. It makes the dependency graph explicit and each class has a single clear responsibility.
- [Risk] Forgetting to call `SetActiveSlot()` before loading → The flow layer always gets the slot from `SlotManager.GetActiveSlot()`, so there's no way to pass a stale value. The state lives in one place.

## Migration Plan

This is a pure refactor with no external API changes. Steps are sequential file edits:

1. Update `SaveBootstrapOptions` (remove `ActiveSlotIndex`)
2. Update `SlotManager` (add active slot, remove `SaveLoadSystem` dependency)
3. Update `SaveLoadSystem` (remove internal state)
4. Update `SceneFlowHandler` and `SaveOnQuitSystem` (use both dependencies)
5. Update `RootLifetimeScope` (wire DI correctly)

Rollback: Simple git revert — no data migration needed.
