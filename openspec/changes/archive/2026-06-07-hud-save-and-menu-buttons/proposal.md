## Why

The gameplay HUD has no way for players to manually save progress or return to the main menu. Players currently have no explicit control over these actions during gameplay — saving only happens automatically on scene exit, and there's no visible "back" option in the HUD.

## What Changes

- **New "Back to Menu" button** in the HUD shell view's top row, publishing `ExitToMenuEvent` (which already triggers an auto-save before scene transition)
- **New "Save" button** in the HUD shell view's top row, publishing a new `ManualSaveEvent` handled by a new `SaveFlowHandler` that saves the active slot via `SaveLoadSystem`
- **New `ManualSaveEvent` command** in the flow events namespace
- **New `SaveFlowHandler`** to handle manual save requests through the VitalRouter pipeline
- **Top action row** added above the existing status bar in `HudShellView`, styled consistently with existing HUD elements

## Capabilities

### New Capabilities

- `hud-shell-controls`: The HUD shell view provides a top action row with Save and Back to Menu buttons, wired through the command routing system for save and navigation actions.

### Modified Capabilities

(none — no existing spec requirements are changing)

## Impact

- **Files changed**: `Events.cs` (new event), new `SaveFlowHandler.cs`, `HudShellView.cs` (buttons + publisher injection), `GameLifetimeScope.cs` (handler registration), `HudShell.uss` (styles for top row)
- **New dependencies in HudShellView**: `ICommandPublisher` injected via VContainer
- **No breaking changes**: All additions are additive to existing systems
