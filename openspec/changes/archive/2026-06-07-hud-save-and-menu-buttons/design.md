## Context

`HudShellView` is a `GameplayViewBase` that builds the entire gameplay HUD: status bar (money, resources, level/XP), tab buttons for sections, and section content areas. It currently receives no `ICommandPublisher` — only state objects and a section factory. Section views get the publisher via `HudSectionFactory`, and `MainMenuView` has its own direct injection.

The save system is fully operational: `SaveLoadSystem.SaveSlotAsync(slotIndex)` handles the full pipeline, `SlotManager.GetActiveSlot()` provides the active slot, and `SceneFlowHandler` already uses both for auto-save on scene exit. `ExitToMenuEvent` is wired end-to-end (publish → handler → save + load menu scene).

## Goals / Non-Goals

**Goals:**

- Add a top action row to `HudShellView` with Save and Back to Menu buttons
- Wire both buttons through the VitalRouter command pipeline (`ICommandPublisher`)
- Introduce `ManualSaveEvent` and `SaveFlowHandler` for explicit save requests
- Keep styling consistent with existing HUD elements

**Non-Goals:**

- Toast/notification system or save feedback UI
- Confirmation dialogs
- Button disable states during async operations
- Auto-save interval timers
- Save slot management UI (create, delete, switch slots)

## Decisions

### Top row placement above status bar

The new buttons live in a dedicated row above the existing status bar, not within it. This keeps the status bar focused on read-only state display and gives action controls their own visual zone.

```
┌──────────────────────────────────────────────┐
│  [Save]                                    [← Menu]   ← hud-actions-row
├──────────────────────────────────────────────┤
│  Money | Resources | Level / XP              │   ← hud-status-bar (unchanged)
├──────────────────────────────────────────────┤
│  [Tabs]                                      │
├──────────────────────────────────────────────┤
│  Section content                             │
└──────────────────────────────────────────────┘
```

### ManualSaveEvent routed through SaveFlowHandler (not direct injection)

The view publishes `ManualSaveEvent` via `ICommandPublisher`, and a dedicated `SaveFlowHandler` handles it by calling `SlotManager.GetActiveSlot()` + `SaveLoadSystem.SaveSlotAsync()`. This is consistent with how every other action flows through the system — section views publish events, handlers orchestrate domain logic. The view has zero knowledge of save infrastructure.

**Alternatives considered:**

- **Direct injection of SaveLoadSystem into HudShellView** — rejected because it breaks the command/event boundary and couples a view to infrastructure
- **Adding save to an existing handler (e.g., SceneFlowHandler)** — rejected because `SaveFlowHandler` is a cleaner separation; scene handling and save orchestration are different concerns

### ICommandPublisher injected into HudShellView via VContainer

The shell already receives dependencies via `[Inject] Construct()`. Adding `ICommandPublisher` follows the same pattern used by `MainMenuView` and all section views. No new registration needed — `ICommandPublisher` is resolved from the VitalRouter instance created in `RootLifetimeScope`.

### Styling matches existing HUD button patterns

The action buttons reuse `.hud-tab-button` visual language (rounded, semi-transparent background, hover state) but with a distinct class `.hud-action-button` so they can be styled independently if needed. The row uses `justify-content: space-between` to push Save left and Menu right.

## Risks / Trade-offs

- **Async save without feedback** — `SaveSlotAsync` is async but the button doesn't indicate progress or success. Risk: user clicks multiple times before I/O completes. Mitigation: file I/O on local storage is fast (< 10ms typical); VitalRouter's `PublishAsync` returns a `UniTask` that serializes commands by default, so rapid clicks will queue rather than overlap.
- **No open questions** — all dependencies exist and are stable.
