## 1. Update SaveBootstrapOptions

- [x] 1.1 Remove `ActiveSlotIndex` field from `SaveBootstrapOptions.cs` (keep only `SlotCount`)

## 2. Refactor SlotManager to own slot state

- [x] 2.1 Add `_activeSlot` field and `GetActiveSlot()` / `SetActiveSlot(int)` methods
- [x] 2.2 Change constructor to take `SaveBootstrapOptions` instead of `int slotCount`, read `.SlotCount` from it
- [x] 2.3 Remove `_saveLoadSystem` field and all delegation calls (`LoadSlotAsync`, `SaveActiveSlotAsync`)
- [x] 2.4 Update class summary comment to reflect new responsibility (slot state management only)

## 3. Make SaveLoadSystem stateless

- [x] 3.1 Remove `_activeSlot` field, `GetActiveSlot()`, and `SetActiveSlot()` methods from `SaveLoadSystem.cs`
- [x] 3.2 Verify `LoadSlotAsync(int)` and `SaveSlotAsync(int)` already accept `slotIndex` as parameter (no signature change needed)

## 4. Update SceneFlowHandler to use both dependencies

- [x] 4.1 Add `SlotManager` constructor parameter alongside existing `SaveLoadSystem`
- [x] 4.2 Replace `_saveLoadSystem.GetActiveSlot()` calls with `_slotManager.GetActiveSlot()` in `On(LoadGameEvent)` and `On(ExitToMenuEvent)`

## 5. Update SaveOnQuitSystem to use SlotManager

- [x] 5.1 Add `SlotManager` constructor parameter alongside existing `SaveLoadSystem`
- [x] 5.2 Replace `_saveLoadSystem.GetActiveSlot()` call with `_slotManager.GetActiveSlot()` in `OnApplicationQuitting`

## 6. Wire DI in RootLifetimeScope

- [x] 6.1 Ensure `SlotManager` is registered as singleton and receives `SaveBootstrapOptions` via constructor injection
- [x] 6.2 Verify no registration passes `int slotCount` or `ActiveSlotIndex` anymore
