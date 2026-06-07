# Economy & Resources Save Modules

## Why

The save system infrastructure (`ISaveModule`, `SaveLoadSystem`, migrations, storage) is complete but has no domain implementations — `IEnumerable<ISaveModule>` resolves to an empty collection, so saving and loading does nothing. Implementing the first two modules proves the pattern works end-to-end and unblocks actual persistence for economy and resource domains.

## What Changes

- **Add** `EconomySaveModule` — serializes/deserializes `EconomyState.Balance` under the `"economy"` JSON key
- **Add** `ResourceSaveModule` — serializes/deserializes `ResourceState.Balances` dictionary under the `"resources"` JSON key
- **Register** both modules as `ISaveModule` in `RootLifetimeScope` so they're discovered by `SaveLoadSystem`

## Capabilities

### New Capabilities

None. This change implements against an existing capability spec.

### Modified Capabilities

- `save-modules`: Concrete implementations of `ISaveModule` are added (Economy and Resources domains). The interface contract already exists; this fulfills it with real domain modules.

## Impact

- **New files**: `Assets/_Game/Scripts/Boot/SaveModules/EconomySaveModule.cs`, `Assets/_Game/Scripts/Boot/SaveModules/ResourceSaveModule.cs`
- **Modified files**: `Assets/_Game/Scripts/Boot/Scopes/RootLifetimeScope.cs` (two new `.As<ISaveModule>()` registrations)
- **Dependencies**: Both modules depend on existing state classes (`EconomyState`, `ResourceState`) — no new external packages required
