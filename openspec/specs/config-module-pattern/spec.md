# Config Module Pattern

## Purpose

Define the config module pattern for feature-driven configuration management. Feature modules implement `IConfigModule` to own their config lifecycle (deserialization, validation, hydration) through a three-phase pipeline, with DTOs colocated in feature modules rather than a centralized shared layer.

## Requirements

### Requirement: Feature modules implement IConfigModule interface

The system SHALL provide an `IConfigModule` interface that defines the config lifecycle contract for feature modules, consisting of a `Key` property and three methods: `Deserialize`, `Validate`, and `Hydrate`.

#### Scenario: Module declares its catalog key

- **WHEN** a feature module implements `IConfigModule`
- **THEN** it exposes a `Key` property returning the string identifier matching the manifest's catalog entry (e.g., `"resources"`)

#### Scenario: Module deserializes JSON into bundle

- **WHEN** `Deserialize(json, bundle)` is called on a module
- **THEN** the module parses the JSON into its own DTO type and stores it in the bundle via `bundle.SetConfig(key, config)`

### Requirement: Config pipeline executes three phases in order

The system SHALL execute the config initialization pipeline in three distinct phases: first deserialize all modules, then validate all modules, then hydrate all modules. No phase begins until the previous phase completes for all modules.

#### Scenario: All modules deserialize before any validation

- **WHEN** the pipeline runs with multiple registered modules
- **THEN** every module's `Deserialize` method is called before any module's `Validate` method is invoked

#### Scenario: Validation errors halt hydration

- **WHEN** any module's `Validate` method adds errors to the error list
- **THEN** the pipeline throws an exception and no module's `Hydrate` method is called

### Requirement: GameCatalogBundle stores configs by string key

The system SHALL provide a `GameCatalogBundle` class that stores deserialized config objects in a dictionary keyed by string, with generic `SetConfig<T>(key, value)` and `GetConfig<T>(key)` methods.

#### Scenario: Module retrieves its own config from bundle

- **WHEN** a module calls `bundle.GetConfig<ResourceCatalogConfig>("resources")` during validation or hydration
- **THEN** the previously deserialized `ResourceCatalogConfig` instance is returned

### Requirement: Modules perform cross-module validation via bundle access

A feature module SHALL be able to access another module's deserialized config from the bundle using that module's string key, enabling cross-reference validation (e.g., generators validating resource references).

#### Scenario: Generator module validates resource references

- **WHEN** `GeneratorConfigModule.Validate()` runs and a generator references an unknown resource ID
- **THEN** the module retrieves the resource catalog via `bundle.GetConfig<ResourceCatalogConfig>("resources")` and adds a validation error for the invalid reference

### Requirement: Config DTOs reside in feature modules, not shared config layer

Each feature module SHALL define its own config DTO classes (e.g., `ResourceCatalogConfig`, `GeneratorCatalogConfig`) within its own namespace, rather than in a centralized shared location.

#### Scenario: Resource catalog config is in Resources module

- **WHEN** the code references `ResourceCatalogConfig`
- **THEN** the type is defined in the `App.Resources.Core` namespace (or equivalent feature module location), not in `App.GameConfig.Core`

### Requirement: Adding a new catalog feature requires changes only within the new feature module

When a new catalog feature is added, the developer SHALL only create or modify files within that feature's module folder and register the new `IConfigModule` implementation. No changes to shared orchestrator classes are required.

#### Scenario: New Quests feature added without touching config system

- **WHEN** a developer adds a "quests" catalog with `QuestCatalogConfig` DTO and `QuestConfigModule : IConfigModule`
- **THEN** no files in the central configuration system (loader, validator, hydrator) are modified; only the new module file is created and registered

### Requirement: GameConfigLoader delegates deserialization to modules

The `GameConfigLoader` SHALL load catalog JSON from addresses specified in the manifest and pass each JSON string to the corresponding module's `Deserialize` method, identified by matching the manifest entry key to the module's `Key` property.

#### Scenario: Loader dispatches JSON to correct module

- **WHEN** the manifest contains a catalog entry with key `"generators"` and address `"configs/generators_catalog.json"`
- **THEN** the loader loads the JSON and calls `Deserialize(json, bundle)` on the module whose `Key` equals `"generators"`

### Requirement: GameConfigValidator delegates validation to modules

The `GameConfigValidator` SHALL iterate over all registered `IConfigModule` instances and invoke each module's `Validate(bundle, errors)` method, collecting all errors before throwing.

#### Scenario: All module validators are invoked

- **WHEN** the validator runs with four registered modules (Resources, Generators, Products, Recipes)
- **THEN** each module's `Validate` method is called with the same bundle and shared error list

### Requirement: GameConfigHydrator delegates hydration to modules

The `GameConfigHydrator` SHALL iterate over all registered `IConfigModule` instances and invoke each module's `Hydrate(bundle)` method, allowing modules to load data into their registries and initialize state.

#### Scenario: Module hydrates its registry from bundle

- **WHEN** `ResourceConfigModule.Hydrate(bundle)` is called
- **THEN** the module retrieves `ResourceCatalogConfig` from the bundle and calls `_resourceRegistry.Load(config)`, then initializes resource state balances
