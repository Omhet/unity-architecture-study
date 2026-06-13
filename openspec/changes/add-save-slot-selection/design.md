# Save Slot Selection Design

## Context

The main menu currently has a single "Start Game" button that publishes `LoadGameEvent`, which the `SceneFlowHandler` handles by loading whatever `_slotManager.GetActiveSlot()` returns (default: slot 0). There's no UI for choosing between save slots, and no way to delete unwanted saves. The `SlotManager` already provides `ListSlotsAsync()`, `SetActiveSlot()`, and `DeleteSlotAsync()` — the infrastructure exists, just not the UI or command layer on top of it.

## Goals / Non-Goals

**Goals:**

- Player can see all save slots with last-played metadata in the main menu
- Player can select any slot (empty or not) to play, which sets it as active and triggers the existing load flow
- Player can delete non-empty slots immediately without confirmation
- Slots panel toggles in-place within MainMenuView (no new popup infrastructure needed)

**Non-Goals:**

- Confirmation dialogs for destructive actions
- Rich slot metadata beyond `LastPlayed` (level, score, playtime, etc.)
- Renaming save slots
- New game vs load game distinction — both use the same `LoadGameEvent` path
- Building a reusable popup/dialog system

## Decisions

### 1. In-place panel toggle within MainMenuView

Rather than building overlay/popup infrastructure, the slots panel lives in the same UIDocument as the main menu. Clicking "Start Game" hides the title+button container and shows the slots panel. A back button reverses this.

**Alternatives considered:**

- _Separate popup view_ — would require new popup infrastructure (modal overlay, focus management, etc.) for a single use case
- _Scene transition to slot select screen_ — overkill for what's essentially a sub-panel of the menu

### 2. SlotManager injected directly into MainMenuView for reading

The view needs `SlotManager.ListSlotsAsync()` to populate the panel. Commands in this codebase are one-way actions (no return values), so querying through VitalRouter doesn't fit. The view injects `SlotManager` directly for reads, and uses commands (`SelectSlotEvent`, `DeleteSlotEvent`) for mutations — keeping the flow layer responsible for state changes while allowing the view to query data directly.

**Alternatives considered:**

- _Query command with callback_ — VitalRouter doesn't support return values; would require introducing a new pattern (Observable, Result<T>, etc.) inconsistent with existing code
- _SlotManager as IProvider interface_ — unnecessary indirection for a single consumer

### 3. Two new commands: SelectSlotEvent and DeleteSlotEvent

`SelectSlotEvent(int slotIndex)` sets the active slot then publishes `LoadGameEvent`, reusing the existing scene transition pipeline. `DeleteSlotEvent(int slotIndex)` calls `SlotManager.DeleteSlotAsync()` directly. Both handled by a new `SlotFlowHandler`.

**Alternatives considered:**

- _Modify LoadGameEvent to accept optional slot index_ — would change the existing event contract and require updating all handlers; cleaner to keep `LoadGameEvent` parameterless and have `SelectSlotEvent` compose it
- _Handle everything in the view without commands_ — bypasses the flow layer, making the view responsible for orchestration logic

### 4. Delete button only shown on non-empty slots

Empty slots have nothing to delete. The `[Delete]` button is conditionally created only when `SlotDescriptor.HasData` is true.

## Risks / Trade-offs

- **Async slot listing during UI build** — `ListSlotsAsync()` is async, so the panel needs to handle the loading state briefly. Mitigation: show a simple "Loading..." text while awaiting, then replace with rows
- **No delete confirmation** — Users might accidentally delete saves. This is an accepted trade-off per requirements; can be added later if needed
- **View depends on SlotManager directly** — Breaks strict command-only communication, but reading slot data through commands doesn't fit the VitalRouter pattern. The dependency is read-only and well-scoped
