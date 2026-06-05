## 1. Storage Layer

- [ ] 1.1 Create `ISaveStorage` interface with `ReadAsync(slotKey)`, `WriteAsync(slotKey, json)`, `DeleteAsync(slotKey)`, `ListSlotsAsync()`, and `CopyToBackupAsync(slotKey)`
- [ ] 1.2 Implement `FileSystemSaveStorage` using `Application.persistentDataPath` with directory creation for `saves/` and `saves/backups/`
- [ ] 1.3 Create `LocalStorageSaveStorage` stub for WebGL (compile-conditionally registered, no-op or localStorage-based)

## 2. Data Models

- [ ] 2.1 Define save file JSON structure: root object with `version` (int), `metadata` (object with `lastPlayed`, `playSessionCount`), and per-domain sections
- [ ] 2.2 Create `SlotDescriptor` data class for slot listing (slot index, hasData, metadata summary)

## 3. Module Interface

- [ ] 3.1 Define `ISaveModule` interface with `Key`, `Serialize()`, `Deserialize(object data)`, and `Validate(object data, List<string> errors)`
- [ ] 3.2 Create placeholder registration in `RootLifetimeScope` for future `IEnumerable<ISaveModule>` consumers (empty collection initially)

## 4. Migration System

- [ ] 4.1 Define `ISaveMigration` interface with `FromVersion`, `ToVersion`, and `Migrate(Dictionary<string, object> saveData)`
- [ ] 4.2 Implement `MigrationChainBuilder` that collects all `ISaveMigration` instances, orders by version, validates no gaps, and returns ordered chain
- [ ] 4.3 Create `CurrentSaveSchemaVersion` constant or service for tracking the latest schema version

## 5. SaveLoadSystem Orchestrator

- [ ] 5.1 Implement `SaveLoadSystem.LoadSlotAsync(slotIndex)`: backup → read → parse JSON → run migrations if needed → validate all sections → dispatch to matching `ISaveModule.Deserialize()`
- [ ] 5.2 Implement `SaveLoadSystem.SaveSlotAsync(slotIndex)`: collect from all `ISaveModule.Serialize()` → merge with version + metadata → write JSON
- [ ] 5.3 Implement active slot tracking (`SetActiveSlot`, `GetActiveSlot`) within the orchestrator

## 6. SlotManager Service

- [ ] 6.1 Implement `SlotManager.ListSlotsAsync()` returning `SlotDescriptor[]` by reading file existence and extracting metadata from each slot JSON
- [ ] 6.2 Implement `SlotManager.DeleteSlotAsync(slotIndex)` removing both main file and backup
- [ ] 6.3 Wire `SlotManager` to delegate load/save operations through `SaveLoadSystem`

## 7. Boot Integration

- [ ] 7.1 Register `ISaveStorage`, `MigrationChainBuilder`, `SaveLoadSystem`, and `SlotManager` in `RootLifetimeScope`
- [ ] 7.2 Create or extend bootstrap options to include slot configuration (slot count, active slot index)
- [ ] 7.3 Hook `SaveLoadSystem.LoadSlotAsync(activeSlot)` into the boot sequence after `GameConfigInitializationSystem.InitializeAsync()` completes

## 8. Save Triggers

- [ ] 8.1 Hook save on scene exit: modify scene flow handler to await `SaveLoadSystem.SaveSlotAsync()` before unloading gameplay scene
- [ ] 8.2 Subscribe to `Application.quitting` event for fire-and-forget safety-net save

## 9. Cleanup

- [ ] 9.1 Remove existing `SaveSystem.cs` (replaced by new modular system)
