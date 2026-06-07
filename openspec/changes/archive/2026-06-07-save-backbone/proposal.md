## Why

The game has no persistence layer. All state (economy, progression, resources, products, generators, recipes, orders, shop, quests, talents) is lost when the application closes. Players need save slots to continue their progress across sessions, with support for multiple saves and data integrity through versioning and backups.

## What Changes

- **New save infrastructure**: `ISaveStorage` platform abstraction (file system + WebGL localStorage stub), `SaveLoadSystem` orchestrator, `SlotManager` service
- **Modular save modules**: `ISaveModule` interface for per-domain serialization/deserialization/validation (interface only; domain implementations deferred to a future change)
- **Migration pipeline**: `ISaveMigration` interface with chain builder for schema version upgrades operating on raw JSON
- **Save slots**: Fixed slot count (configurable, default 4), each slot is a separate JSON file with metadata header and per-domain sections
- **Backup strategy**: Before loading a slot, copy to backup file for corruption recovery
- **Boot integration**: `LoadAndApplyAsync()` called after config hydration; save triggered on scene exit and app quit
- **Slot management API**: List slots, get metadata, load slot, save slot, delete slot (data layer only; UI deferred)

## Capabilities

### New Capabilities

- `save-storage`: Platform-agnostic save file I/O with slot support, backup creation, and metadata tracking
- `save-modules`: Modular per-domain serialization interface (`ISaveModule`) for serializing/deserializing game state to/from JSON sections
- `save-migrations`: Versioned migration pipeline that transforms raw save JSON between schema versions
- `save-orchestration`: Orchestration of load/save lifecycle including backup, migration, validation, module dispatch, and boot integration

### Modified Capabilities

(No existing capabilities are modified. This is purely additive infrastructure.)

## Impact

- **New files**: `ISaveStorage`, `FileSystemSaveStorage`, `LocalStorageSaveStorage` (stub), `ISaveModule`, `ISaveMigration`, `MigrationChainBuilder`, `SaveLoadSystem`, `SlotManager`, save data models, boot options
- **Modified files**: `RootLifetimeScope.cs` (register new services), scene flow handler (hook save on exit)
- **Existing `SaveSystem.cs`**: Replaced by the new modular system (deprecated/removed)
- **Dependencies**: None added. Uses existing Newtonsoft.Json, Cysharp.Threading.Tasks, VContainer
