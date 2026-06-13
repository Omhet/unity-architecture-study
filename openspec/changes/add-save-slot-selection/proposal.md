# Add Save Slot Selection

## Why

The main menu currently has a single "Start Game" button that loads the default active slot with no player choice. Players need to see their existing saves, pick which one to continue, and manage (delete) slots they no longer want.

## What Changes

- **Main menu now shows a save slots panel** — clicking "Start Game" reveals an in-place panel listing all configured save slots instead of immediately loading the game
- **Each slot row displays metadata** — slot index, last played timestamp (or "(empty)" for unused slots), plus action buttons
- **Slot Play button sets active slot and loads** — clicking Play on any slot sets it as the active slot via `SlotManager.SetActiveSlot()` then triggers the existing `LoadGameEvent` flow
- **Slot Delete button removes saves** — clicking Delete on a non-empty slot immediately deletes that slot's save data (no confirmation dialog)
- **Two new commands** — `SelectSlotEvent` (set active + load) and `DeleteSlotEvent` (delete slot), with a new `SlotFlowHandler` to route them

## Capabilities

### New Capabilities

- `save-slot-selection`: Main menu save slots panel UI, including slot listing, Play/Delete actions, and in-place visibility toggle within MainMenuView

### Modified Capabilities

- `save-orchestration`: The active slot selection flow changes — previously the flow layer always used the default active slot; now the UI explicitly sets it via `SlotManager.SetActiveSlot()` before loading. Adds a new command-driven path for slot selection and deletion through the flow layer.

## Impact

- **Assets/\_Game/Scripts/View/Menu/MainMenuView.cs** — Major changes: add slots panel, inject `SlotManager`, toggle visibility
- **Assets/\_Game/Scripts/Flow/Events/Events.cs** — New `SelectSlotEvent` and `DeleteSlotEvent` command structs
- **New file: SlotFlowHandler.cs** — Handles slot selection and deletion commands
- **openspec/specs/save-orchestration/spec.md** — Delta spec documenting the new slot selection/deletion requirements
