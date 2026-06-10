## Why

The save system currently uses untyped `object` parameters in `ISaveModule`, causing runtime cast exceptions when deserializing JSON. Newtonsoft.Json deserializes nested objects as `JObject`, not concrete types, breaking modules that expect `Dictionary<string, int>` or other specific types. This creates type-unsafe code with inconsistent patterns across modules.

## What Changes

- **Introduce `SaveDataBundle`**: Type-safe container mirroring `GameCatalogBundle` pattern from config system
- **Refactor `ISaveModule` interface**: Replace untyped methods with bundle-based API (Serialize, Deserialize, Validate, Apply)
- **Introduce explicit save DTO pattern**: Each module defines local DTO classes for its save schema (e.g., `ResourceSaveData`)
- **Update `SaveLoadSystem`**: Change orchestration to use `JObject` for migrations and pass `JToken` sections directly to modules (no `JObject -> string -> deserialize` round-trip)
- **Update all existing save modules**: Convert to new bundle-based pattern with explicit DTOs
- **Update `ISaveMigration` interface**: Change from `Dictionary<string, object>` to `JObject` for clearer structure manipulation

## Capabilities

### New Capabilities

- `save-data-bundle`: Type-safe storage container for save data with generic Get/Set methods
- `typed-save-modules`: Save module interface using bundle pattern for strongly typed module APIs and explicit DTO contracts

### Modified Capabilities

<!-- No existing save specs to modify - this is establishing the pattern -->

## Impact

**Code Changes:**

- `ISaveModule.cs` - Interface signature changes (**BREAKING** for all modules)
- `SaveLoadSystem.cs` - Orchestration logic refactored for bundle pattern
- `SaveDataBundle.cs` - New file
- `ISaveMigration.cs` - Change parameter type from Dictionary to JObject
- All save modules (ResourceSaveModule, EconomySaveModule, etc.) - Refactored to use bundle + DTOs

**Dependencies:**

- Continues to use Newtonsoft.Json for serialization
- Maintains compatibility with existing save file JSON structure (no data migration needed)

**Benefits:**

- Strongly typed module API and DTO contracts reduce runtime cast errors and make schemas explicit
- Avoids unnecessary JSON section re-serialization during load
- Consistent pattern across all save modules
- Mirrors proven config system architecture
- Easier to add new save modules with clear DTO contracts

**Safety Clarification:**

- DTO + bundle pattern improves compile-time ergonomics inside module code (typed access, IntelliSense)
- JSON-to-DTO structural correctness remains a runtime concern without additional schema/contract validation
