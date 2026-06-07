## ADDED Requirements

### Requirement: Migration Interface with Version Chain

The system SHALL define an `ISaveMigration` interface that transforms raw save JSON from one schema version to the next.

#### Scenario: Migration declares version range

- **WHEN** a migration is implemented
- **THEN** it SHALL expose `FromVersion` and `ToVersion` properties defining which versions it bridges

#### Scenario: Migration transforms raw JSON structure

- **WHEN** `Migrate(saveData)` is called with a dictionary representing the save file root
- **THEN** the migration SHALL mutate the dictionary in place to produce the target version's structure

### Requirement: Chain Builder Validates and Orders Migrations

The system SHALL provide a mechanism that collects all registered migrations, orders them by version, and validates there are no gaps in the chain.

#### Scenario: Valid chain constructed

- **WHEN** migrations exist for every step from file version to current version (e.g., v1→v2, v2→v3)
- **THEN** the chain builder SHALL return an ordered sequence of migrations to apply

#### Scenario: Gap in chain detected

- **WHEN** a migration is missing for a required step (e.g., v1→v2 exists but v2→v3 does not)
- **THEN** the system SHALL throw a descriptive error identifying the missing migration step

### Requirement: Migrations Applied Before Deserialization

Migrations SHALL be applied to raw save data before any domain module's `Deserialize` method is called.

#### Scenario: Old save migrated then loaded

- **WHEN** a save file with an older version number is loaded
- **THEN** all required migrations SHALL run first, producing current-version JSON, before modules deserialize the data

#### Scenario: Current version skip migrations

- **WHEN** a save file's version matches the current schema version
- **THEN** no migrations SHALL be applied and deserialization proceeds directly
