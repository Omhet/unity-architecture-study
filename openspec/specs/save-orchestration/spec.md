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
- **THEN** `SaveLoadSystem.LoadSlotAsync(activeSlot)` SHALL be called before gameplay begins

### Requirement: Save Triggered on Scene Exit

The system SHALL save the active slot when the player exits the gameplay scene.

#### Scenario: Save on scene transition

- **WHEN** the player triggers a scene exit from the gameplay scene
- **THEN** `SaveLoadSystem.SaveSlotAsync(activeSlot)` SHALL be awaited before the scene is unloaded

### Requirement: Save Triggered on Application Quit

The system SHALL attempt to save when the application is closing.

#### Scenario: Save on quit event

- **WHEN** `Application.quitting` fires
- **THEN** the system SHALL trigger a save of the active slot (fire-and-forget, not awaited)

### Requirement: Active Slot Tracking

The system SHALL track which slot is currently active for load and save operations.

#### Scenario: Active slot set before game start

- **WHEN** a player selects a slot to play
- **THEN** the system SHALL record that slot index as the active slot for subsequent save/load calls
