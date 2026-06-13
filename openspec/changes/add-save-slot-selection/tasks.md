## 1. Add Slot Commands

- [x] 1.1 Add `SelectSlotEvent(int slotIndex)` struct to `Events.cs` implementing `ICommand`
- [x] 1.2 Add `DeleteSlotEvent(int slotIndex)` struct to `Events.cs` implementing `ICommand`

## 2. Create Slot Flow Handler

- [x] 2.1 Create `SlotFlowHandler.cs` with `[Routes]` attribute, injecting `SlotManager` and `ICommandPublisher`
- [x] 2.2 Implement `On(SelectSlotEvent)` — call `_slotManager.SetActiveSlot(slotIndex)`, then publish `LoadGameEvent`
- [x] 2.3 Implement `On(DeleteSlotEvent)` — await `_slotManager.DeleteSlotAsync(slotIndex)`

## 3. Build Slots Panel in MainMenuView

- [x] 3.1 Inject `SlotManager` into `MainMenuView` via `[Inject]` alongside existing `ICommandPublisher`
- [x] 3.2 Refactor `BuildView()` to create a "main panel" container (title + Start Game button) and a hidden "slots panel" container with a header ("Save Slots") and back button
- [x] 3.3 Update `HandlePlayClicked()` to hide the main panel, show the slots panel, and call `RefreshSlotsPanelAsync()`
- [x] 3.4 Implement `OnBackClicked()` to show the main panel and hide the slots panel
- [x] 3.5 Implement `RefreshSlotsPanelAsync()` — await `_slotManager.ListSlotsAsync()`, clear existing rows, and build a slot row for each descriptor with metadata text, Play button (publishes `SelectSlotEvent`), and Delete button (publishes `DeleteSlotEvent`, only if `HasData`)
