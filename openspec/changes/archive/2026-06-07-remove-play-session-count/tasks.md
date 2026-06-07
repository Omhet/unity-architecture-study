## 1. Remove from Models

- [x] 1.1 Remove `PlaySessionCount` property and `[JsonProperty]` attribute from `SaveMetadata` in `SaveFileData.cs`
- [x] 1.2 Remove `PlaySessionCount` property, `[JsonProperty]`, and session count text from `Summary` in `SlotDescriptor.cs`

## 2. Simplify SaveLoadSystem

- [x] 2.1 Remove post-load increment-and-write block from `LoadSlotAsync()` (lines ~99–109)
- [x] 2.2 Remove pre-save read-for-count logic from `SaveSlotAsync()` and simplify metadata to only include `lastPlayed`

## 3. Simplify SlotManager

- [x] 3.1 Remove `playSessionCount` local variable, parsing, and assignment in `ListSlotsAsync()`
