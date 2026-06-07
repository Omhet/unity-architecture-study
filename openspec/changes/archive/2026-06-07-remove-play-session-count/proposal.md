## Why

`playSessionCount` tracks how many times a slot has been loaded, adding two extra storage round-trips per load/save cycle (a pre-save read and a post-load write) for minimal value — it's a misleadingly named counter that doesn't represent playtime or progress.

## What Changes

- **Remove** `playSessionCount` from metadata in save files
- **Remove** the post-load increment-and-write side effect in `SaveLoadSystem.LoadSlotAsync()` (load becomes pure read)
- **Remove** the pre-save read-for-count in `SaveLoadSystem.SaveSlotAsync()` (save becomes pure write)
- **Remove** `PlaySessionCount` from `SlotDescriptor` and simplify its `Summary` property
- **Remove** `PlaySessionCount` from `SaveMetadata`
- **Remove** session count parsing from `SlotManager.ListSlotsAsync()`
- Existing save files on disk may retain the orphaned key — it will be ignored, not migrated

## Capabilities

### New Capabilities

<!-- None — this is a removal/simplification -->

### Modified Capabilities

- `save-storage`: Metadata no longer includes `playSessionCount`; only `lastPlayed` remains in the metadata section
- `save-orchestration`: Load pipeline no longer has side-effect writes; save pipeline no longer requires pre-read for session count

## Impact

- **Files affected**:
  - `SaveLoadSystem.cs` — removes post-load write and pre-save read logic
  - `SlotManager.cs` — removes session count parsing from ListSlotsAsync
  - `SlotDescriptor.cs` — removes PlaySessionCount property, simplifies Summary
  - `SaveFileData.cs` — removes PlaySessionCount from SaveMetadata
- **No breaking API changes** — `playSessionCount` was internal implementation detail, not exposed through public APIs
- **Existing save files**: orphaned key in JSON is harmless and will naturally disappear as slots are overwritten
