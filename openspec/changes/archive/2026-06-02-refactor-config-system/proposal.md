## Why

The config system (loader, validator, hydrator) violates SOLID principles — every feature addition requires modifying shared configuration classes with switch statements, hardcoded DTOs, and domain-specific validation logic. Adding a new catalog feature touches 6+ files in the central config layer instead of just the feature module itself.

## What Changes

- **Introduce `IConfigModule` interface** — each feature module implements this interface to own its config lifecycle (deserialize, validate, hydrate)
- **Move config DTOs from shared `GameplayCatalogs.cs` into respective feature modules** — e.g., `ResourceCatalogConfig` lives in `Resources/`, not `App.GameConfig.Core`
- **Replace `GameCatalogBundle` with a generalized bundle** using `Dictionary<string, object>` that stores deserialized configs by key, eliminating the god-object pattern
- **Refactor `GameConfigLoader`** to delegate deserialization to modules via `IConfigModule.Deserialize()` instead of switch-case dispatching
- **Refactor `GameConfigValidator`** to delegate validation to modules via `IConfigModule.Validate()`, with cross-module validation handled by modules accessing other configs from the bundle using string keys
- **Refactor `GameConfigHydrator`** to delegate hydration to modules via `IConfigModule.Hydrate()` instead of knowing about every registry and state
- **Three-phase pipeline** — deserialize all → validate all → hydrate all, eliminating ordering concerns between modules
- **Remove `GameplayCatalogs.cs`** (DTOs distributed to feature modules)
- **Remove switch-case dispatching** from the loader

## Capabilities

### New Capabilities

- `config-module-pattern`: Feature modules own their config lifecycle through `IConfigModule` interface with three-phase pipeline (deserialize, validate, hydrate), eliminating central knowledge of domain-specific configs

### Modified Capabilities

_(None — no existing specs to modify)_

## Impact

- **Files removed**: `GameplayCatalogs.cs`, switch-case logic in loader/validator/hydrator
- **Files modified**: `GameConfigLoader.cs`, `GameConfigValidator.cs`, `GameConfigHydrator.cs`, `GameConfigInitializationSystem.cs`
- **Files created**: `IConfigModule.cs`, `GameCatalogBundle.cs` (generalized), one `<Feature>ConfigModule.cs` per feature (Resources, Generators, Products, Recipes)
- **DTOs moved**: `ResourceCatalogConfig` → `Resources/`, `GeneratorCatalogConfig` → `Generators/`, etc.
- **Assembly references**: Feature modules will depend on the config interface; no new external dependencies
