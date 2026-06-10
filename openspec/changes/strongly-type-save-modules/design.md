## Context

The current save system uses an untyped `ISaveModule` interface with `object` parameters. When `SaveLoadSystem` deserializes JSON with `JsonConvert.DeserializeObject<Dictionary<string, object>>()`, nested objects become `JObject` instances, not concrete types. This causes `InvalidCastException` when modules try to cast to expected types like `Dictionary<string, int>`.

**Current inconsistency:**

- `ResourceSaveModule` attempts direct cast to `Dictionary<string, int>` → crashes
- `EconomySaveModule` casts to `JObject` and navigates tokens → works but couples to JSON.NET

The project already has a proven pattern for this problem: the config system uses `GameCatalogBundle` with typed Get/Set methods, allowing modules to work with concrete types while the bundle handles storage.

## Goals / Non-Goals

**Goals:**

- Eliminate runtime type cast exceptions in save modules
- Provide strongly typed module APIs and explicit DTO contracts for save data operations
- Establish consistent pattern mirroring the existing config system architecture
- Support explicit save DTOs (data transfer objects) for each module's schema
- Enable clean structure manipulation in migrations using JObject
- Maintain transaction semantics: validate all before applying any

**Non-Goals:**

- Changing save file JSON structure or format (no data migration needed)
- Removing Newtonsoft.Json dependency
- Making modules JSON-library agnostic
- Supporting asynchronous validation or partial saves

## Decisions

### Decision 1: Mirror Config System Pattern with SaveDataBundle

**Choice:** Introduce `SaveDataBundle` class with generic `SetData<T>()` and `GetData<T>()` methods, identical to `GameCatalogBundle`.

**Rationale:**

- Proven pattern already in production for config loading
- Provides type safety through generics while hiding `object` storage
- Familiar to developers who know the config system
- Minimal code (~10 lines) for significant value

**Alternatives considered:**

- Generic `ISaveModule<TData>` interface: Can't enumerate heterogeneous collection of modules
- Direct JObject coupling: Ties all modules to JSON.NET implementation details
- Two-phase deserialization with `GetDataType()`: Extra complexity with double serialization

### Decision 2: Split Module Interface into Serialize/Deserialize/Validate/Apply

**Choice:** New `ISaveModule` signature:

```csharp
public interface ISaveModule
{
    string Key { get; }
    void Serialize(SaveDataBundle bundle);           // Domain → Bundle
    void Deserialize(JToken section, SaveDataBundle bundle);  // JSON token → Bundle
    void Validate(SaveDataBundle bundle, List<string> errors);    // Check typed data
    void Apply(SaveDataBundle bundle);               // Bundle → Domain
}
```

**Rationale:**

- **Deserialize**: Each module knows its own type and converts its own `JToken` section to typed DTO without string re-serialization
- **Validate**: Read-only validation on typed data, collect ALL errors before any mutation
- **Apply**: Separate phase ensures atomic transaction semantics
- Mirrors config system's Deserialize → Validate → Hydrate pipeline

**Alternatives considered:**

- Single `Load(object data)` method: Loses transaction semantics, can't validate-all-before-apply
- Keeping `object Serialize()` return: Works but less explicit than bundle pattern

### Decision 3: Require Explicit DTOs for All Modules

**Choice:** Each module defines a save data class (e.g., `ResourceSaveData`) in the same file as the module, even for simple single-field schemas.

**Rationale:**

- Explicit schema contract makes data shape visible
- Enables IntelliSense and compile-time checking
- Consistent pattern reduces cognitive load
- Easy to evolve (add fields) without breaking other code
- Matches config system convention (`GeneratorCatalogConfig`)

**DTO Placement:** Same file as module, separate top-level class (not nested), named `<Module>SaveData`

**Alternatives considered:**

- Anonymous objects for simple modules: Inconsistent, harder to refactor
- Nested classes: More verbose to reference, less clear
- Separate files: Overkill for typically small DTOs

### Decision 4: Use JObject/JToken through migrations and module boundary

**Choice:** Change `ISaveMigration.Migrate(JObject saveData)` from `Dictionary<string, object>`.

**Rationale:**

- JObject is what `JsonConvert.DeserializeObject<Dictionary<string, object>>()` actually produces for nested objects
- Provides clean API for structure manipulation (rename fields, add sections, transform nested data)
- Type-safe token access with `JTokenType` checks
- Designed specifically for JSON transformation tasks

**Migration Flow:**

1. Parse JSON to `JObject`
2. Run migrations on `JObject` (structure changes)
3. Pass section `JToken`s to modules, modules convert tokens to typed DTOs
4. Validate typed data, apply to domain

**Alternatives considered:**

- Keep Dictionary: Already broken (nested objects are JObject anyway)
- Custom migration DTO types: Overkill, migrations are one-off transformations

### Decision 5: Throw on Missing Save Data

**Choice:** `Bundle.GetData<T>()` throws exception if key not found. Modules are not invoked for missing sections.

**Rationale:**

- Missing section indicates corrupted or incorrectly migrated save file
- Fail-fast prevents propagating bad state
- Matches config system behavior
- Migrations are responsible for adding new sections with defaults

**Load Flow:**

- If section exists in JSON → Deserialize → Validate → Apply
- If section missing → Skip module entirely, preserve domain defaults

**Alternatives considered:**

- Return null and let modules handle: Moves validation complexity to every module
- Auto-apply defaults: Masks migration bugs, could corrupt state

## Risks / Trade-offs

**[Risk] Breaking change to all existing save modules**
→ Mitigation: Only 2 modules exist (ResourceSaveModule, EconomySaveModule). Refactor both as part of this change.

**[Risk] JSON.NET coupling in all modules**
→ Acknowledged trade-off: Simplicity over abstraction. Config system already established this dependency. Migration cost to alternative serializer would be one-time refactor.

**[Risk] Bundle uses runtime cast internally**
→ Acknowledged: Type safety is at module API level, not storage. This is same pattern as GameCatalogBundle and considered acceptable.

**[Risk] DTO typing can be overstated as full compile-time safety**
→ Mitigation: Treat JSON-to-DTO shape checks as runtime concerns; rely on strict validation/tests and optional schema/contract checks if needed.

**[Trade-off] More verbose than current object-based approach**
→ Benefit: Explicitness prevents bugs. DTO + 4 methods vs 3 methods, but eliminates entire class of runtime errors.

**[Trade-off] Bundle.GetData throws on missing key**
→ Benefit: Fail-fast semantics. Avoids silent bugs from missing data. Migrations must handle schema evolution.

## Migration Plan

**Implementation Order:**

1. Create `SaveDataBundle.cs`
2. Update `ISaveModule.cs` interface
3. Update `ISaveMigration.cs` interface (Dictionary → JObject)
4. Refactor `SaveLoadSystem.cs` orchestration
5. Refactor existing save modules with DTOs
6. Update unit tests

**Rollback Strategy:**

- No save file format changes; rollback is just code revert
- Existing save files compatible before and after change

**Testing:**

- Verify round-trip: save → load produces identical state
- Test migration chain with JObject changes
- Verify validation errors prevent state corruption
- Test missing section handling

## Open Questions

None - architecture decision is complete based on existing config system pattern.
