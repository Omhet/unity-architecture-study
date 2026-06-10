## MODIFIED Requirements

### Requirement: ProgressionSaveModule serializes level and xp to plain data

The system SHALL provide a `ProgressionSaveModule` that implements `ISaveModule` with Key `"progression"`, serializing the current `ProgressionState.Level.Value` and `ProgressionState.Xp.Value` to a plain object. The module SHALL NOT serialize `NextLevelXp` — it is derived data available from the progression registry.

#### Scenario: Serialize returns level and xp only

- **WHEN** `Serialize()` is called on `ProgressionSaveModule`
- **THEN** it SHALL return an object with `level` and `xp` properties, and SHALL NOT contain a `nextLevelXp` property

#### Scenario: Deserialize applies level and xp from data

- **WHEN** `Deserialize(data)` is called with valid progression save data containing `level` and `xp` fields
- **THEN** the module SHALL set `_state.Level.Value` and `_state.Xp.Value` to the deserialized values

#### Scenario: Old save file with extra nextLevelXp field loads without error

- **WHEN** `Deserialize(data)` is called with progression save data that also contains a `nextLevelXp` field from an older save format
- **THEN** the module SHALL ignore the `nextLevelXp` field and apply only `level` and `xp`

#### Scenario: Validate rejects level less than 1

- **WHEN** `Validate(data, errors)` is called with progression save data where `level < 1`
- **THEN** the errors list SHALL contain a validation error message
