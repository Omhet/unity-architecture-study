## ADDED Requirements

### Requirement: ISaveModule Interface Definition

The system SHALL define an `ISaveModule` interface with methods for serializing domain state to plain data, deserializing plain data into domain state, and validating save data.

#### Scenario: Module declares its JSON key

- **WHEN** a save module is implemented
- **THEN** it SHALL expose a `Key` property that matches the JSON section name in the save file

#### Scenario: Serialize extracts plain data from runtime state

- **WHEN** `Serialize()` is called on a module
- **THEN** it SHALL return a plain serializable object (not containing ReactiveProperty or ObservableCollections) representing the current domain state

#### Scenario: Deserialize applies data to runtime state

- **WHEN** `Deserialize(data)` is called with valid plain data
- **THEN** the module SHALL apply the data to its domain's runtime state objects

#### Scenario: Validate checks data integrity before applying

- **WHEN** `Validate(data, errors)` is called with save data
- **THEN** the module SHALL populate the errors list with any validation failures found in the data

### Requirement: Module Discovery via DI Container

Save modules SHALL be registered in the VContainer DI container and discoverable as `IEnumerable<ISaveModule>`.

#### Scenario: Modules injected into orchestrator

- **WHEN** the save orchestration system resolves `IEnumerable<ISaveModule>`
- **THEN** it SHALL receive all registered save module instances

### Requirement: No Data Means No Deserialize Call

When a save file contains no data for a domain's key, that domain's `Deserialize` method SHALL NOT be called.

#### Scenario: Missing section skipped

- **WHEN** the save JSON does not contain a section matching a module's `Key`
- **THEN** that module's `Deserialize()` SHALL NOT be invoked and the domain state SHALL retain its default values
