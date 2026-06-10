## ADDED Requirements

### Requirement: Module declares save key

Each ISaveModule implementation SHALL declare a unique string key identifying its save data section.

#### Scenario: Unique module keys

- **WHEN** multiple save modules are registered
- **THEN** each module has a distinct Key property value

### Requirement: Serialize domain state to bundle

The ISaveModule SHALL provide a Serialize method that writes domain state to SaveDataBundle.

#### Scenario: Serialize current state

- **WHEN** SaveLoadSystem calls `module.Serialize(bundle)`
- **THEN** module writes current domain state as typed DTO to bundle using module's Key

#### Scenario: Serialize with no data

- **WHEN** domain has no meaningful state to save
- **THEN** module MAY skip calling SetData on bundle

### Requirement: Deserialize JSON token section to bundle

The ISaveModule SHALL provide a Deserialize method that converts a JSON token section and writes typed data to SaveDataBundle.

#### Scenario: Deserialize valid JSON

- **WHEN** SaveLoadSystem calls `module.Deserialize(sectionToken, bundle)` with a valid section token
- **THEN** module converts the token to typed DTO and stores in bundle using SetData

#### Scenario: Deserialize invalid JSON structure

- **WHEN** module receives a token whose structure cannot be converted to its DTO schema
- **THEN** module throws exception with clear conversion error

### Requirement: Validate typed data before mutation

The ISaveModule SHALL provide a Validate method that checks typed data from bundle without side effects.

#### Scenario: Validate correct data

- **WHEN** SaveLoadSystem calls `module.Validate(bundle, errors)`
- **THEN** module retrieves typed data from bundle and adds no errors

#### Scenario: Validate invalid data

- **WHEN** module finds data that violates constraints
- **THEN** module adds descriptive error messages to errors list without modifying domain state

#### Scenario: Validate missing data

- **WHEN** module's key is not present in bundle
- **THEN** module is not invoked for validation (SaveLoadSystem skips missing sections)

### Requirement: Apply typed data to domain state

The ISaveModule SHALL provide an Apply method that writes validated typed data to domain state.

#### Scenario: Apply validated data

- **WHEN** SaveLoadSystem calls `module.Apply(bundle)` after successful validation
- **THEN** module retrieves typed data from bundle and updates domain state

#### Scenario: Apply is only called after validation

- **WHEN** any module fails validation
- **THEN** no module's Apply method is called (transaction semantics)

### Requirement: Use explicit DTO for save schema

Each ISaveModule implementation SHALL define an explicit DTO class representing its save data schema.

#### Scenario: DTO defines schema

- **WHEN** module implementation is created
- **THEN** module includes a SaveData class (e.g., ResourceSaveData) defining the save schema

#### Scenario: DTO is local to module

- **WHEN** examining module source file
- **THEN** DTO class is defined in the same file as the module

#### Scenario: DTO naming convention

- **WHEN** module is named XyzSaveModule
- **THEN** DTO is named XyzSaveData

### Requirement: Fail fast on corrupted save data

The save system SHALL throw exceptions for corrupted or incorrectly migrated save files.

#### Scenario: Missing expected section after migration

- **WHEN** module expects data but section is missing from JSON after migrations
- **THEN** system throws exception indicating migration failure

#### Scenario: Invalid data structure

- **WHEN** JSON section structure doesn't match module's DTO schema
- **THEN** deserialization throws exception with type mismatch details
