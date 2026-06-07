# Save Orchestration

## Purpose

Orchestrate the complete save and load pipelines including backup, migration, validation, serialization/deserialization coordination, and application lifecycle integration.

## Requirements

### Requirement: Load Slot Orchestration

The system SHALL provide a `SaveLoadSystem` that orchestrates the complete load pipeline: backup, read, migrate, validate, and deserialize.

#### Scenario: Full load pipeline executes

- **WHEN** `LoadSlotAsync(slotIndex)` is called for a slot with existing data
- **THEN** the system SHALL execute in order: (1) backup current file, (2) read JSON from storage, (3) run migrations if version mismatch, (4) validate all domain sections, (5) dispatch to matching `ISaveModule.Deserialize()`

#### Scenario: Empty slot load is no-op

- **WHEN** `LoadSlotAsync(slotIndex)` is called for a slot with no save file
- **THEN** the system SHALL complete without error and all domain states SHALL retain their default values

### Requirement: Save Slot Orchestration

The system SHALL provide a save pipeline that collects data from all modules, merges into a single JSON structure, and writes to storage.

#### Scenario: Full save pipeline executes

- **WHEN** `SaveSlotAsync(slotIndex)` is called
- **THEN** the system SHALL call `Serialize()` on all registered `ISaveModule` instances, merge results with version and metadata into a single JSON object, and write to storage

### Requirement: Validation Errors Prevent Load

If validation fails during load, the system SHALL report errors rather than applying partial or corrupt data.

#### Scenario: Validation failure reported

- **WHEN** any `ISaveModule.Validate()` populates the errors list during load
- **THEN** the system SHALL NOT call `Deserialize()` on any module and SHALL return/report the validation errors

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
