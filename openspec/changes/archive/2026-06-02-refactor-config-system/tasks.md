## 1. Create foundation types

- [x] 1.1 Create `IConfigModule` interface with `Key`, `Deserialize(json, bundle)`, `Validate(bundle, errors)`, `Hydrate(bundle)` members in `App.Systems.Configuration` namespace
- [x] 1.2 Create generalized `GameCatalogBundle` class with `Manifest` property and `SetConfig<T>(key, config)` / `GetConfig<T>(key)` methods using `Dictionary<string, object>`

## 2. Move DTOs to feature modules

- [x] 2.1 Move `ResourceDefinition` and `ResourceCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Resources/`
- [x] 2.2 Move `GeneratorDefinition` and `GeneratorCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Generators/`
- [x] 2.3 Move `ProductDefinition` and `ProductCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Products/`
- [x] 2.4 Move `RecipeDefinition` and `RecipeCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Recipes/`
- [x] 2.5 Keep `GameConfigManifest` and `ConfigCatalogEntry` in `_GameConfig` folder (they're shared infrastructure, not feature-specific)

## 3. Implement feature config modules

- [x] 3.1 Create `ResourceConfigModule : IConfigModule` with `Key = "resources"`, deserialize logic, unique-ID validation, registry load + state initialization
- [x] 3.2 Create `GeneratorConfigModule : IConfigModule` with `Key = "generators"`, deserialize logic, unique-ID + resource-reference validation via bundle, registry load + state initialization
- [x] 3.3 Create `ProductConfigModule : IConfigModule` with `Key = "products"`, deserialize logic, unique-ID validation, registry load
- [x] 3.4 Create `RecipeConfigModule : IConfigModule` with `Key = "recipes"`, deserialize logic, unique-ID + product/resource-reference validation via bundle, registry load + state initialization

## 4. Refactor orchestrators

- [x] 4.1 Refactor `GameConfigLoader.LoadAsync()` to accept `IEnumerable<IConfigModule>`, load manifest and catalog JSONs, then dispatch each JSON to the matching module's `Deserialize` method by key
- [x] 4.2 Refactor `GameConfigValidator.ValidateOrThrow()` to iterate over modules calling `Validate(bundle, errors)` for each, collecting all errors before throwing
- [x] 4.3 Refactor `GameConfigHydrator.Hydrate()` to iterate over modules calling `Hydrate(bundle)` for each

## 5. Update initialization system and registration

- [x] 5.1 Update `GameConfigInitializationSystem` to use the three-phase pipeline (deserialize → validate → hydrate) via the refactored orchestrators
- [x] 5.2 Register all four config modules as `IConfigModule` in `RootLifetimeScope`, inject them into loader/validator/hydrator constructors

## 6. Remove legacy code

- [x] 6.1 Delete `GameplayCatalogs.cs` (DTOs moved, manifest types remain in `_GameConfig`)
- [x] 6.2 Remove old strongly-typed properties from any remaining bundle class and delete if empty
- [x] 6.3 Remove switch-case dispatching logic from loader, domain-specific validation methods from validator, and hydration methods from hydrator

## 7. Verify and test

- [x] 7.1 Build the project and resolve all compilation errors (namespace imports, assembly references)
- [ ] 7.2 Run the game in editor and verify config initialization completes without errors
- [ ] 7.3 Verify cross-module validation still catches invalid references (e.g., generator with unknown resource ID)
