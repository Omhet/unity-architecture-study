## ADDED Requirements

### Requirement: EconomySaveModule serializes balance to plain data

The system SHALL provide an `EconomySaveModule` that implements `ISaveModule` with Key `"economy"`, serializing the current `EconomyState.Balance.Value` to a plain anonymous object.

#### Scenario: Serialize returns balance value

- **WHEN** `Serialize()` is called on `EconomySaveModule`
- **THEN** it SHALL return an object with a `balance` property equal to `_state.Balance.Value`

#### Scenario: Deserialize applies balance from data

- **WHEN** `Deserialize(data)` is called with valid economy save data containing a `balance` field
- **THEN** the module SHALL set `_state.Balance.Value` to the deserialized value

#### Scenario: Validate rejects negative balance

- **WHEN** `Validate(data, errors)` is called with economy save data where `balance < 0`
- **THEN** the errors list SHALL contain a validation error message

### Requirement: ResourceSaveModule serializes balances dictionary to plain data

The system SHALL provide a `ResourceSaveModule` that implements `ISaveModule` with Key `"resources"`, serializing all entries in `ResourceState.Balances` to a plain `Dictionary<string, int>`.

#### Scenario: Serialize returns all balance entries

- **WHEN** `Serialize()` is called on `ResourceSaveModule`
- **THEN** it SHALL return a dictionary containing all key-value pairs from `_state.Balances` as a plain `Dictionary<string, int>`

#### Scenario: Deserialize merges saved values into state

- **WHEN** `Deserialize(data)` is called with valid resource save data (a dictionary of resourceId → amount)
- **THEN** the module SHALL call `_state.SetAmount()` for each entry, merging saved values without clearing existing keys not present in the save data

#### Scenario: Validate rejects negative resource amounts

- **WHEN** `Validate(data, errors)` is called with resource save data where any value is less than 0
- **THEN** the errors list SHALL contain a validation error message identifying the invalid resource key

### Requirement: Save modules are registered in DI container as ISaveModule

Both `EconomySaveModule` and `ResourceSaveModule` SHALL be registered in `RootLifetimeScope` with `Lifetime.Singleton` and exposed as `ISaveModule`.

#### Scenario: SaveLoadSystem receives both modules via DI

- **WHEN** `SaveLoadSystem` resolves `IEnumerable<ISaveModule>` from the container
- **THEN** it SHALL include instances of both `EconomySaveModule` and `ResourceSaveModule`
