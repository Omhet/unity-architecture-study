## Why

Slot count and active slot state are scattered across three places — `SaveBootstrapOptions` (registered but never consumed), `SlotManager` (hardcoded default parameter for slot count), and `SaveLoadSystem` (hardcoded field default for active slot). None of these values are wired together, making the configuration dead code and the runtime state fragile. This refactor establishes `SlotManager` as the single source of truth for all slot-related state and makes `SaveLoadSystem` a stateless pipeline.

## What Changes

- **`SlotManager`** becomes the sole owner of `_slotCount` (from `SaveBootstrapOptions`) and `_activeSlot` (mutable runtime state), exposing `GetActiveSlot()` and `SetActiveSlot(int)`.
- **`SlotManager`** no longer depends on or delegates to `SaveLoadSystem` — it manages slot state only.
- **`SaveLoadSystem`** removes `_activeSlot`, `GetActiveSlot()`, and `SetActiveSlot()`. All public methods already accept `slotIndex` as a parameter; internal state is eliminated.
- **`SceneFlowHandler`** depends on both `SlotManager` (for active slot) and `SaveLoadSystem` (for pipeline calls), getting the slot index from one and passing it to the other.
- **`SaveOnQuitSystem`** same pattern — reads active slot from `SlotManager`, passes to `SaveLoadSystem`.
- **`SaveBootstrapOptions`** removes `ActiveSlotIndex` (runtime state, not boot config); keeps only `SlotCount`.
- **`RootLifetimeScope`** wires `SaveBootstrapOptions` into `SlotManager` via DI.

## Capabilities

### New Capabilities

(none)

### Modified Capabilities

- `save-orchestration`: Active slot tracking ownership moves from `SaveLoadSystem` to `SlotManager`. The load/save pipeline methods remain the same (`slotIndex` parameter), but internal state and getter/setter for active slot are removed from `SaveLoadSystem` and live in `SlotManager` instead.

## Impact

- **Files changed**: `SlotManager.cs`, `SaveLoadSystem.cs`, `SceneFlowHandler.cs`, `SaveOnQuitSystem.cs`, `SaveBootstrapOptions.cs`, `RootLifetimeScope.cs`
- **Breaking internal API**: `SaveLoadSystem.SetActiveSlot()` and `SaveLoadSystem.GetActiveSlot()` are removed. Callers use `SlotManager` instead.
- **No external API change**: The save/load pipeline methods (`LoadSlotAsync(int)`, `SaveSlotAsync(int)`) keep the same signature.
