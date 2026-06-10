## ADDED Requirements

### Requirement: Type-safe data storage

The SaveDataBundle SHALL provide type-safe storage for save data sections using generic methods.

#### Scenario: Store typed data

- **WHEN** module calls `bundle.SetData<ResourceSaveData>(key, data)`
- **THEN** data is stored with full type information preserved

#### Scenario: Retrieve typed data

- **WHEN** module calls `bundle.GetData<ResourceSaveData>(key)`
- **THEN** data is returned as the correct type without casting

### Requirement: Throw on missing key

The SaveDataBundle SHALL throw an exception when attempting to retrieve data for a non-existent key.

#### Scenario: Missing key access

- **WHEN** module calls `bundle.GetData<T>(key)` for a key that doesn't exist
- **THEN** system throws an exception with clear error message

#### Scenario: Check key existence

- **WHEN** module calls `bundle.HasData(key)`
- **THEN** system returns true if key exists, false otherwise

### Requirement: Support any reference type

The SaveDataBundle SHALL support storage of any reference type (class instances).

#### Scenario: Store complex DTO

- **WHEN** module stores a DTO with nested collections and objects
- **THEN** bundle preserves the complete object graph

#### Scenario: Store simple DTO

- **WHEN** module stores a DTO with primitive fields
- **THEN** bundle preserves all field values

### Requirement: Independent storage per key

The SaveDataBundle SHALL maintain independent storage for each key without interference.

#### Scenario: Multiple modules store data

- **WHEN** multiple modules call SetData with different keys
- **THEN** each module can retrieve only its own data by key

#### Scenario: Overwrite existing data

- **WHEN** module calls SetData twice for the same key
- **THEN** second call overwrites first value completely
