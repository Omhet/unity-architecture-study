## 1. Storage Layer

- [x] 1.1 Create `ISaveStorage` interface with `ReadAsync(slotKey)`, `WriteAsync(slotKey, json)`, `DeleteAsync(slotKey)`, `ListSlotsAsync()`, and `CopyToBackupAsync(slotKey)`
- [x] 1.2 Implement `FileSystemSaveStorage` using `Application.persistentDataPath` with directory creation for `saves/` and `saves/backups/`
- [x] 1.3 Create `LocalStorageSaveStorage` stub for WebGL (compile-conditionally registered, no-op or localStorage-based)

## 2. Data Models

- [x] 2.1 Define save file JSON structure: root object with `version` (int), `metadata` (object with `lastPlayed`, `playSessionCount`), and per-domain sections
- [x] 2.2 Create `SlotDescriptor` data class for slot listing (slot index, hasData, metadata summary)

## 3. Module Interface

- [x] 3.1 Define `ISaveModule` interface with `Key`, `Serialize()`, `Deserialize(object data)`, and `Validate(object data, List<string> errors)`
- [x] 3.2 Create placeholder registration in `RootLifetimeScope` for future `IEnumerable<ISaveModule>` consumers (empty collection initially)

## 4. Migration System

- [x] 4.1 Define `ISaveMigration` interface with `FromVersion`, `ToVersion`, and `Migrate(Dictionary<string, object> saveData)`
- [x] 4.2 Implement `MigrationChainBuilder` that collects all `ISaveMigration` instances, orders by version, validates no gaps, and returns ordered chain
- [x] 4.3 Create `CurrentSaveSchemaVersion` constant or service for tracking the latest schema version

## 5. SaveLoadSystem Orchestrator

- [x] 5.1 Implement `SaveLoadSystem.LoadSlotAsync(slotIndex)`: backup → read → parse JSON → run migrations if needed → validate all sections → dispatch to matching `ISaveModule.Deserialize()`
- [x] 5.2 Implement `SaveLoadSystem.SaveSlotAsync(slotIndex)`: collect from all `ISaveModule.Serialize()` → merge with version + metadata → write JSON
- [x] 5.3 Implement active slot tracking (`SetActiveSlot`, `GetActiveSlot`) within the orchestrator

## 6. SlotManager Service

- [x] 6.1 Implement `SlotManager.ListSlotsAsync()` returning `SlotDescriptor[]` by reading file existence and extracting metadata from each slot JSON
- [x] 6.2 Implement `SlotManager.DeleteSlotAsync(slotIndex)` removing both main file and backup
- [x] 6.3 Wire `SlotManager` to delegate load/save operations through `SaveLoadSystem`

## 7. Boot Integration

- [x] 7.1 Register `ISaveStorage`, `MigrationChainBuilder`, `SaveLoadSystem`, and `SlotManager` in `RootLifetimeScope`
- [x] 7.2 Create or extend bootstrap options to include slot configuration (slot count, active slot index)
- [x] 7.3 Hook `SaveLoadSystem.LoadSlotAsync(activeSlot)` into the boot sequence after `GameConfigInitializationSystem.InitializeAsync()` completes

## 8. Save Triggers

- [x] 8.1 Hook save on scene exit: modify scene flow handler to await `SaveLoadSystem.SaveSlotAsync()` before unloading gameplay scene
- [x] 8.2 Subscribe to `Application.quitting` event for fire-and-forget safety-net save

## 9. Cleanup

- [x] 9.1 Remove existing `SaveSystem.cs` (replaced by new modular system)
