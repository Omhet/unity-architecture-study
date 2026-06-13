## MODIFIED Requirements

### Requirement: Boot Integration After Config Hydration

The save load operation SHALL be invoked after config hydration completes during application boot, or when the player explicitly selects a slot from the main menu.

#### Scenario: Load called after config in boot sequence

- **WHEN** the application boots and `GameConfigInitializationSystem.InitializeAsync()` completes
- **THEN** the flow layer SHALL call `SlotManager.GetActiveSlot()` then pass the result to `SaveLoadSystem.LoadSlotAsync(slotIndex)` before gameplay begins

#### Scenario: Load triggered by slot selection from menu

- **WHEN** the player selects a slot from the main menu save slots panel
- **THEN** the system SHALL first call `SlotManager.SetActiveSlot(slotIndex)`, then publish `LoadGameEvent` which triggers config initialization, save loading from the active slot, and scene transition

## ADDED Requirements

### Requirement: Slot Selection via Command

The flow layer SHALL provide a command-driven path for selecting a save slot before loading.

#### Scenario: SelectSlotEvent sets active slot and loads

- **WHEN** `SelectSlotEvent(slotIndex)` is published
- **THEN** the system SHALL call `SlotManager.SetActiveSlot(slotIndex)` and SHALL publish `LoadGameEvent` to trigger the load pipeline

### Requirement: Slot Deletion via Command

The flow layer SHALL provide a command-driven path for deleting a save slot.

#### Scenario: DeleteSlotEvent removes slot data

- **WHEN** `DeleteSlotEvent(slotIndex)` is published
- **THEN** the system SHALL call `SlotManager.DeleteSlotAsync(slotIndex)` to remove the slot's save data from storage
