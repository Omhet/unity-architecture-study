## 1. Add ManualSaveEvent

- [x] 1.1 Add `ManualSaveEvent` readonly struct implementing `ICommand` to `Events.cs` in `Assets/_Game/Scripts/Flow/Events/`

## 2. Create SaveFlowHandler

- [x] 2.1 Create `SaveFlowHandler.cs` in `Assets/_Game/Scripts/Flow/Handlers/` with `[Routes]` and `[Route(CommandOrdering.Drop)]` attributes
- [x] 2.2 Inject `SlotManager` and `SaveLoadSystem` into handler constructor
- [x] 2.3 Implement `On(ManualSaveEvent _)` that calls `_saveLoadSystem.SaveSlotAsync(_slotManager.GetActiveSlot())`

## 3. Register SaveFlowHandler in GameLifetimeScope

- [x] 3.1 Add `routing.Map<SaveFlowHandler>()` to the VitalRouter configuration in `GameLifetimeScope.Configure()`
- [x] 3.2 Add `using App.Flow.Handlers;` if not already present

## 4. Add ICommandPublisher injection to HudShellView

- [x] 4.1 Add `ICommandPublisher _publisher` field to `HudShellView`
- [x] 4.2 Add `ICommandPublisher publisher` parameter to the `[Inject] Construct()` method and assign it
- [x] 4.3 Add `using VitalRouter;` if not already present

## 5. Build action row in HudShellView

- [x] 5.1 Add `_saveButton` and `_menuButton` field declarations (type `Button`)
- [x] 5.2 Create a `BuildActionRow()` method that creates a `VisualElement` with class `hud-actions-row`, containing two buttons: "Save" (left) and "← Menu" (right), both with class `hud-action-button`
- [x] 5.3 Wire Save button click to publish `ManualSaveEvent` via `_publisher.PublishAsync()`
- [x] 5.4 Wire Menu button click to publish `ExitToMenuEvent` via `_publisher.PublishAsync()`
- [x] 5.5 Call `BuildActionRow()` in `BuildView()` and add the returned element to the shell, before the status bar

## 6. Clean up action row buttons in DisposeView

- [x] 6.1 Null out `_saveButton` and `_menuButton` references in `DisposeView()`

## 7. Add USS styles for action row

- [x] 7.1 Add `.hud-actions-row` class to `HudShell.uss` with `flex-direction: row`, `justify-content: space-between`, `align-items: center`, and `margin-bottom: 8px`
- [x] 7.2 Add `.hud-action-button` class matching `.hud-tab-button` conventions (border-radius, background-color, color, hover state)
