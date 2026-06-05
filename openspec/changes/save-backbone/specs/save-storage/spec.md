## ADDED Requirements

### Requirement: Platform-Agnostic Storage Interface

The system SHALL provide an `ISaveStorage` interface that abstracts platform-specific file I/O operations for save data.

#### Scenario: Desktop platform uses file system storage

- **WHEN** the application runs on a desktop platform (Windows, macOS, Linux)
- **THEN** `FileSystemSaveStorage` is registered as the implementation of `ISaveStorage`

#### Scenario: WebGL platform uses local storage

- **WHEN** the application runs on WebGL
- **THEN** `LocalStorageSaveStorage` is registered as the implementation of `ISaveStorage`

### Requirement: Slot-Based Save Files

The system SHALL support a configurable number of fixed save slots, where each slot corresponds to a separate JSON file on disk.

#### Scenario: Default slot count is configured

- **WHEN** no explicit slot count is configured
- **THEN** the system SHALL use 4 slots as the default

#### Scenario: Slot files are named predictably

- **WHEN** a save is written to slot N
- **THEN** the file SHALL be stored at `saves/slot_{N}.json` relative to `Application.persistentDataPath`

### Requirement: Backup Creation Before Load

The system SHALL create a backup copy of a slot's save file before loading it, to enable recovery from corruption.

#### Scenario: Backup created on load

- **WHEN** a player loads a slot that contains an existing save file
- **THEN** the system SHALL copy the current file to `saves/backups/slot_{N}_backup.json` before reading save data

#### Scenario: No backup needed for empty slot

- **WHEN** a player loads a slot with no existing save file
- **THEN** the system SHALL NOT create a backup file

### Requirement: Save File Metadata

Each save file SHALL contain a metadata section with version and session information.

#### Scenario: Metadata written on save

- **WHEN** a save is written to disk
- **THEN** the JSON root SHALL include a `version` field (integer) and a `metadata` object with `lastPlayed` timestamp

#### Scenario: Version field tracks schema version

- **WHEN** save data is serialized
- **THEN** the `version` field SHALL reflect the current save schema version number

### Requirement: Slot Listing and Metadata Retrieval

The system SHALL provide an API to list all available slots with their metadata without loading full save data.

#### Scenario: List all slots

- **WHEN** the slot listing API is called
- **THEN** the system SHALL return a collection of slot descriptors containing slot index, whether data exists, and metadata summary

### Requirement: Slot Deletion

The system SHALL support deleting a slot's save file, removing both the main file and its backup.

#### Scenario: Delete populated slot

- **WHEN** a populated slot is deleted
- **THEN** both `slot_{N}.json` and `slot_{N}_backup.json` SHALL be removed from disk

#### Scenario: Delete empty slot

- **WHEN** an empty slot (no save file exists) is deleted
- **THEN** the operation SHALL succeed silently with no error
