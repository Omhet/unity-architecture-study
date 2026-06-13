# Quest Save

## ADDED Requirements

### Requirement: QuestSaveModule serializes quest progress data

The system SHALL provide a `QuestSaveModule` implementing `ISaveModule` that serializes per-quest completion and claimable state from `QuestState.ProgressMap`.

#### Scenario: Serialize quests with mixed states

- **WHEN** `Serialize(bundle)` is called and `ProgressMap` contains entries for completed, in-progress, and unstarted quests
- **THEN** the module creates a `QuestSaveData` DTO containing progress entries for all map keys
- **AND** each entry includes the quest's `IsClaimable.Value` and `IsCompleted.Value` as plain booleans

#### Scenario: Serialize empty progress map

- **WHEN** `Serialize(bundle)` is called and `ProgressMap` is empty
- **THEN** the module creates a `QuestSaveData` with an empty progress dictionary

### Requirement: QuestSaveModule deserializes save data to bundle

The system SHALL provide a `Deserialize` method that converts JSON quest progress data into a typed DTO and stores it in the bundle.

#### Scenario: Deserialize valid quest progress

- **WHEN** `Deserialize(section, bundle)` receives a valid JSON token with quest progress entries
- **THEN** the module converts the token to `QuestSaveData` and stores it in the bundle

#### Scenario: Deserialize invalid data throws

- **WHEN** `Deserialize(section, bundle)` receives a token that cannot be converted to `QuestSaveData`
- **THEN** the module throws an `InvalidOperationException` with a descriptive message

### Requirement: QuestSaveModule applies saved progress to reactive properties

The system SHALL provide an `Apply` method that sets the `.Value` of reactive properties in `ProgressMap` entries from deserialized save data.

#### Scenario: Apply restores completed state

- **WHEN** `Apply(bundle)` is called with save data marking a quest as completed
- **THEN** the corresponding `QuestProgressData.IsCompleted.Value` is set to `true`

#### Scenario: Apply restores claimable state

- **WHEN** `Apply(bundle)` is called with save data marking a quest as claimable
- **THEN** the corresponding `QuestProgressData.IsClaimable.Value` is set to `true`

#### Scenario: Apply skips missing quests

- **WHEN** `Apply(bundle)` is called with save data containing entries for quests not in `ProgressMap`
- **THEN** those entries are silently ignored (no exception)

### Requirement: QuestSaveModule validates quest progress data

The system SHALL provide a `Validate` method that checks quest progress data integrity.

#### Scenario: Validate correct data

- **WHEN** `Validate(bundle, errors)` is called with valid quest save data
- **THEN** no errors are added to the errors list

### Requirement: QuestSaveModule uses "quests" as its save key

The system SHALL declare `"quests"` as the module's unique JSON key.

#### Scenario: Module key is quests

- **WHEN** accessing `QuestSaveModule.Key`
- **THEN** it returns `"quests"`
