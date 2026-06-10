## Context

`ProgressionState` currently holds three reactive properties: `Level`, `Xp`, and `NextLevelXp`. The `NextLevelXp` value represents the XP threshold required to reach the next level — but this is entirely determined by the player's current level and the progression catalog (loaded from JSON config into `ProgressionRegistry`). It is derived data, not independent state.

Currently:

- `ProgressionService.AddXp()` computes the threshold from the registry, then writes it back into `_progressionState.NextLevelXp`
- `HudShellView` subscribes to both `Xp` and `NextLevelXp` reactive properties for UI binding
- `ProgressionSaveModule` serializes `NextLevelXp` to save files and restores it on load
- Config changes (balance tweaks) can cause saved values to drift from catalog values

## Goals / Non-Goals

**Goals:**

- Eliminate redundant storage of derived data in state and save files
- Enforce clean separation: state = "where is the player?", config/registry = "what does each level require?"
- Ensure `NextLevelXp` always reflects the current catalog (no drift from balance changes)

**Non-Goals:**

- Changing the progression algorithm or level-up logic
- Modifying how the registry loads or queries data
- Introducing new reactive patterns or computed property abstractions

## Decisions

### Decision: UI queries registry directly, no computed reactive wrapper

The HUD needs `NextLevelXp` to display "XP: 45 / 100". Rather than creating a computed reactive property that derives the threshold from level + registry, the view injects `ProgressionRegistry` and calls `GetNextLevelXpForLevel()` on demand when `Level` or `Xp` changes.

**Rationale:** The registry lookup is O(log n) over a tiny sorted list (typically 3-10 entries). Adding a computed reactive layer adds complexity without meaningful performance benefit. The view already depends on domain services for section factories and publishers — adding the registry is consistent with this pattern.

### Decision: Existing save files with `nextLevelXp` field are ignored on load

The save module simply stops reading/writing `nextLevelXp`. Old save files will have an extra JSON field that is deserialized but discarded. No migration logic needed.

**Rationale:** Backward compatibility is free — the JSON field just gets ignored. On next save, it won't be written. This is a natural deprecation over one save cycle.

## Risks / Trade-offs

- [UI reactivity] The HUD previously subscribed to two independent reactive properties (`Xp` and `NextLevelXp`). Now it subscribes to `Level` and `Xp`, and derives the threshold imperatively from the registry on each callback. This is slightly less "pure" reactive, but the trade-off is cleaner state semantics. → The registry is immutable after load, so this is safe — no race conditions or stale reads.
- [Save file migration] Old save files have an extra field. → Harmless — ignored during deserialization.

## Migration Plan

No runtime migration needed. The change is:

1. Remove `NextLevelXp` from state and save module
2. On load, old saves are read without the field (no error)
3. On next save, the field is not written
4. Registry is already loaded before save apply, so all lookups work
