## MODIFIED Requirements

### Requirement: Active Slot Tracking

The `SlotManager` SHALL be the single source of truth for slot count and active slot state. It receives `SlotCount` from `SaveBootstrapOptions` at construction time and manages `_activeSlot` as mutable runtime state. `SaveLoadSystem` SHALL NOT track the active slot internally.

#### Scenario: Active slot set before game start

- **WHEN** a player selects a slot to play
- **THEN** `SlotManager.SetActiveSlot(slotIndex)` SHALL record that slot index as the active slot for subsequent save/load calls

#### Scenario: Flow layer reads active slot from SlotManager

- **WHEN** the flow layer needs to load or save
- **THEN** it SHALL call `SlotManager.GetActiveSlot()` to obtain the current slot index and pass it to `SaveLoadSystem.LoadSlotAsync(slotIndex)` or `SaveLoadSystem.SaveSlotAsync(slotIndex)`

#### Scenario: Slot count injected from SaveBootstrapOptions

- **WHEN** `SlotManager` is constructed via DI
- **THEN** it SHALL receive `SaveBootstrapOptions` and use `.SlotCount` as the immutable slot count for validation and iteration

### Requirement: Boot Integration After Config Hydration

The save load operation SHALL be invoked after config hydration completes during application boot.

#### Scenario: Load called after config in boot sequence

- **WHEN** the application boots and `GameConfigInitializationSystem.InitializeAsync()` completes
- **THEN** the flow layer SHALL call `SlotManager.GetActiveSlot()` then pass the result to `SaveLoadSystem.LoadSlotAsync(slotIndex)` before gameplay begins

### Requirement: Save Triggered on Scene Exit

The system SHALL save the active slot when the player exits the gameplay scene.

#### Scenario: Save on scene transition

- **WHEN** the player triggers a scene exit from the gameplay scene
- **THEN** the flow layer SHALL call `SlotManager.GetActiveSlot()` then pass the result to `SaveLoadSystem.SaveSlotAsync(slotIndex)` and await it before the scene is unloaded

### Requirement: Save Triggered on Application Quit

The system SHALL attempt to save when the application is closing.

#### Scenario: Save on quit event

- **WHEN** `Application.quitting` fires
- **THEN** the system SHALL call `SlotManager.GetActiveSlot()` and trigger a fire-and-forget save via `SaveLoadSystem.SaveSlotAsync(slotIndex)`
