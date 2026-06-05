## Context

The game currently has a `SaveSystem` class that provides generic JSON file I/O but is disconnected from the rest of the application — registered in DI but never called. All 10 state objects (economy, progression, resources, products, generators, recipes, orders, shop, quests, talents) use reactive properties and observable collections that are not directly serializable. The config system already has a proven modular pattern (`IConfigModule`) with loader → validator → hydrator orchestration.

## Goals / Non-Goals

**Goals:**

- Modular save infrastructure mirroring the existing `IConfigModule` pattern
- Platform-agnostic storage abstraction (file system now, WebGL localStorage later)
- Save slots with backup strategy for corruption recovery
- Versioned migration pipeline operating on raw JSON
- Boot integration: load after config hydration, save on scene exit and app quit
- Slot management API (data layer only) for future UI consumption

**Non-Goals:**

- Domain `ISaveModule` implementations (deferred to a separate change)
- Slot selection UI / popup (deferred to a separate change)
- Cloud saves
- Save file compression or encryption

## Decisions

### Decision 1: Modular ISaveModule Pattern (Not Central DTO)

**Choice:** Each domain implements `ISaveModule` with `Serialize()`/`Deserialize()` — no central `GameStateDTO` class.

**Rationale:** Mirrors the existing `IConfigModule` pattern that's already proven in this codebase. Adding a new domain means adding one module, zero changes to orchestration. A central DTO would become a god-object touched by every domain change.

**Alternatives considered:**

- Single `GameStateDTO` with all fields — simpler initially but couples all domains together
- Service-based save where each service knows how to save itself — leaks persistence concern into services

### Decision 2: Migrations Operate on Raw JSON, Not Domain Objects

**Choice:** `ISaveMigration.Migrate(Dictionary<string, object> saveData)` transforms the raw JSON structure.

**Rationale:** Migrations are pure data transforms independent of runtime state or services. They can be tested in isolation (input JSON → output JSON). If a domain is removed, its migration still works because it operates on structure, not objects.

**Alternatives considered:**

- Domain-specific migrations — would require all domain code to be available during load, creating coupling
- Manual version checks per module — scattered logic, hard to reason about chains

### Decision 3: Single JSON File Per Slot with Per-Domain Sections

**Choice:** One file per slot (`slot_0.json`), with top-level keys matching domain modules. A `version` and `metadata` section at the root.

**Rationale:** Simple for players to manage (one file = one save). The modular structure inside means domains don't step on each other. Per-domain files were considered but add unnecessary complexity for a game of this scale — the JSON is small (<50KB).

### Decision 4: Backup Before Load, Not After Save

**Choice:** When loading a slot, copy current file to backup first. On successful exit, save overwrites the main file (backup remains as fallback).

**Rationale:** If the game crashes or corrupts state during a session, the backup is the last known good state from before that session. Backing up after save doesn't protect against in-session corruption.

### Decision 5: ISaveStorage Platform Abstraction

**Choice:** Interface with `ReadAsync(slotKey)`, `WriteAsync(slotKey, json)`, etc., implemented by `FileSystemSaveStorage` (desktop/mobile) and `LocalStorageSaveStorage` (WebGL stub).

**Rationale:** WebGL doesn't have file I/O — needs localStorage or IndexedDB. The abstraction makes this a swap at registration time, no conditional compilation in business logic.

### Decision 6: Save Triggers on Scene Exit, Not Every Action

**Choice:** Primary save trigger is scene transition (via `SceneFlowHandler`). Safety net is `Application.quitting` event.

**Rationale:** Saving after every action is chatty and unnecessary for this game genre. Scene exit is a natural checkpoint where the player has committed to leaving gameplay. `Application.quitting` catches unexpected closes but is fire-and-forget (can't await).

### Decision 7: SlotManager as Data Service, Not UI Controller

**Choice:** `SlotManager` provides data operations (list slots, get metadata, delete) without any UI concerns.

**Rationale:** Keeps the backbone change focused on infrastructure. The slot selection popup is a separate concern that consumes this service. Following the same separation as how registries/services are structured today.

## Risks / Trade-offs

- **Corrupted save file with no valid backup** → Mitigation: validation step catches bad JSON; if both main and backup fail, offer "start fresh" to user (in UI change)
- **Migration chain has a gap** (e.g., v1→v3 exists but v2→v3 missing) → Mitigation: `MigrationChainBuilder` validates the chain before applying; throws descriptive error if gaps found
- **WebGL localStorage size limits (~5MB)** → Mitigation: save files are small (<50KB), 4 slots + backups = ~400KB total, well within limits
- **Save on quit is fire-and-forget** → Mitigation: primary save happens on scene exit (awaited); quit handler is just a safety net
- **ReactiveProperty and ObservableCollections not serializable** → Mitigation: `ISaveModule` implementations extract plain values; this change only provides the interface

## Open Questions

- How many slots should be the default? (Proposal says 4, but this is a game design decision)
- Should slot metadata include a player-settable name? (Useful for UI, can be added later)
- Should we support importing/exporting save files? (Probably not needed initially)
