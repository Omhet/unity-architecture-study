## Context

The config system currently has three orchestrator classes (`GameConfigLoader`, `GameConfigValidator`, `GameConfigHydrator`) that contain domain-specific knowledge about every feature module (Resources, Generators, Products, Recipes). Adding a new catalog feature requires modifying all three orchestrators plus the shared DTO file and bundle class — 6+ files in the central config layer for a single feature change.

Current data flow:

```
Loader (switch-case deserialize) → Validator (domain-specific checks) → Hydrator (registry/state calls)
     knows about types              knows about domains               knows about registries
```

All DTOs live in `GameplayCatalogs.cs` under `App.GameConfig.Core`, and `GameCatalogBundle` has strongly-typed properties for each catalog type.

## Goals / Non-Goals

**Goals:**

- Feature modules own their config lifecycle (deserialize, validate, hydrate) via a shared interface
- Adding/removing a feature requires changes only within that feature module's folder
- Three-phase pipeline eliminates ordering concerns between modules during validation
- Central orchestrators are generic — they iterate over modules without knowing domain details

**Non-Goals:**

- Changing how configs are stored on disk (JSON files, manifest structure remain the same)
- Introducing convention-based discovery or reflection-based module scanning
- Changing VContainer registration patterns beyond adding `IConfigModule` registrations
- Migrating away from Newtonsoft.Json

## Decisions

### 1. Three-phase pipeline: Deserialize → Validate → Hydrate

**Why**: Eliminates ordering concerns. All modules deserialize first (no dependencies needed), then all validate (all data available in bundle), then all hydrate. A module can cross-validate against another module's config without worrying about hydration order.

**Alternatives considered:**

- Single-phase `Apply()` — simpler but requires explicit execution ordering, which we explicitly rejected
- Two-phase (Deserialize + Apply) — merges validate/hydrate but doesn't provide meaningful benefit over three phases

### 2. `IConfigModule` interface with four members

```csharp
public interface IConfigModule {
    string Key { get; }
    void Deserialize(string json, GameCatalogBundle bundle);
    void Validate(GameCatalogBundle bundle, List<string> errors);
    void Hydrate(GameCatalogBundle bundle);
}
```

**Why**: Each member maps to a pipeline phase. The `Key` property lets the loader match manifest entries to modules and lets other modules reference this module's config in the bundle for cross-validation.

### 3. Generalized `GameCatalogBundle` with `Dictionary<string, object>`

```csharp
public class GameCatalogBundle {
    public GameConfigManifest Manifest;
    private readonly Dictionary<string, object> _configs = new();

    public void SetConfig<T>(string key, T config) => _configs[key] = config;
    public T GetConfig<T>(string key) => (T)_configs[key];
}
```

**Why**: Strongly-typed `GameCatalogBundle` with per-catalog properties violates OCP. A dictionary allows modules to store/retrieve configs without the bundle class changing. Runtime casting is acceptable since each module controls both the put and get of its own key/type, and cross-module access uses string keys (which is explicit coupling).

**Alternatives considered:**

- Store raw JSON strings instead — would require double deserialization (validate + hydrate), unnecessary performance cost
- Keep strongly-typed bundle — defeats the purpose; still requires modifying the class per feature

### 4. DTOs move to feature modules

`ResourceCatalogConfig` moves from `App.GameConfig.Core` to `App.Resources.Core`, etc. Each module owns its data shapes entirely.

**Why**: Information Expert principle — the Resources module knows what a resource config looks like, not a shared config layer.

### 5. Cross-module validation via bundle access with string keys

```csharp
// GeneratorConfigModule validates generator→resource references:
public void Validate(GameCatalogBundle bundle, List<string> errors) {
    var generators = bundle.GetConfig<GeneratorCatalogConfig>(Key);
    var resources  = bundle.GetConfig<ResourceCatalogConfig>("resources");

    foreach (var g in generators.Generators) {
        if (!resources.Resources.Any(r => r.Id == g.ResourceId)) ...
    }
}
```

**Why**: Modules need access to other modules' deserialized configs for cross-reference validation. Using the bundle with string keys is explicit coupling — it's visible and intentional, not hidden behind DI magic. The string key `"resources"` is a convention documented by the `Key` property on each module.

### 6. Explicit registration in `RootLifetimeScope`

Modules are registered as `IConfigModule` implementations via VContainer. The initialization system receives `IEnumerable<IConfigModule>` from the container.

**Why**: Compile-time safety, visible dependency graph, no reflection overhead. Registration order is not significant since the three-phase pipeline handles ordering concerns.

## Risks / Trade-offs

- [Runtime casting in bundle] → Each module controls its own key/type pair; cross-module access requires knowing both the string key and DTO type, which is explicit coupling by design
- [Modules depend on other modules' DTO types for cross-validation] → Unavoidable when validating references between domains; kept minimal (only `GetConfig<T>()` call, no behavioral dependency)
- [Breaking change to config system internals] → No external API changes; this is an internal refactor with no impact on game behavior

## Migration Plan

1. Create `IConfigModule` interface and generalized `GameCatalogBundle`
2. Move DTOs from `GameplayCatalogs.cs` to respective feature modules
3. Implement `<Feature>ConfigModule` for each existing feature (Resources, Generators, Products, Recipes)
4. Refactor orchestrators to iterate over `IEnumerable<IConfigModule>`
5. Register modules in `RootLifetimeScope`, remove old registrations
6. Delete `GameplayCatalogs.cs`, old strongly-typed bundle, switch-case logic

Rollback: standard git revert; no data migration needed since config files on disk are unchanged.
