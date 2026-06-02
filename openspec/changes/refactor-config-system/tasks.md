## 1. Create foundation types

- [ ] 1.1 Create `IConfigModule` interface with `Key`, `Deserialize(json, bundle)`, `Validate(bundle, errors)`, `Hydrate(bundle)` members in `App.Systems.Configuration` namespace
- [ ] 1.2 Create generalized `GameCatalogBundle` class with `Manifest` property and `SetConfig<T>(key, config)` / `GetConfig<T>(key)` methods using `Dictionary<string, object>`

## 2. Move DTOs to feature modules

- [ ] 2.1 Move `ResourceDefinition` and `ResourceCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Resources/`
- [ ] 2.2 Move `GeneratorDefinition` and `GeneratorCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Generators/`
- [ ] 2.3 Move `ProductDefinition` and `ProductCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Products/`
- [ ] 2.4 Move `RecipeDefinition` and `RecipeCatalogConfig` from `GameplayCatalogs.cs` to `Assets/_Game/Scripts/Core/Recipes/`
- [ ] 2.5 Keep `GameConfigManifest` and `ConfigCatalogEntry` in `_GameConfig` folder (they're shared infrastructure, not feature-specific)

## 3. Implement feature config modules

- [ ] 3.1 Create `ResourceConfigModule : IConfigModule` with `Key = "resources"`, deserialize logic, unique-ID validation, registry load + state initialization
- [ ] 3.2 Create `GeneratorConfigModule : IConfigModule` with `Key = "generators"`, deserialize logic, unique-ID + resource-reference validation via bundle, registry load + state initialization
- [ ] 3.3 Create `ProductConfigModule : IConfigModule` with `Key = "products"`, deserialize logic, unique-ID validation, registry load
- [ ] 3.4 Create `RecipeConfigModule : IConfigModule` with `Key = "recipes"`, deserialize logic, unique-ID + product/resource-reference validation via bundle, registry load + state initialization

## 4. Refactor orchestrators

- [ ] 4.1 Refactor `GameConfigLoader.LoadAsync()` to accept `IEnumerable<IConfigModule>`, load manifest and catalog JSONs, then dispatch each JSON to the matching module's `Deserialize` method by key
- [ ] 4.2 Refactor `GameConfigValidator.ValidateOrThrow()` to iterate over modules calling `Validate(bundle, errors)` for each, collecting all errors before throwing
- [ ] 4.3 Refactor `GameConfigHydrator.Hydrate()` to iterate over modules calling `Hydrate(bundle)` for each

## 5. Update initialization system and registration

- [ ] 5.1 Update `GameConfigInitializationSystem` to use the three-phase pipeline (deserialize → validate → hydrate) via the refactored orchestrators
- [ ] 5.2 Register all four config modules as `IConfigModule` in `RootLifetimeScope`, inject them into loader/validator/hydrator constructors

## 6. Remove legacy code

- [ ] 6.1 Delete `GameplayCatalogs.cs` (DTOs moved, manifest types remain in `_GameConfig`)
- [ ] 6.2 Remove old strongly-typed properties from any remaining bundle class and delete if empty
- [ ] 6.3 Remove switch-case dispatching logic from loader, domain-specific validation methods from validator, and hydration methods from hydrator

## 7. Verify and test

- [ ] 7.1 Build the project and resolve all compilation errors (namespace imports, assembly references)
- [ ] 7.2 Run the game in editor and verify config initialization completes without errors
- [ ] 7.3 Verify cross-module validation still catches invalid references (e.g., generator with unknown resource ID)
