## 1. Create SaveModules folder structure

- [x] 1.1 Create `Assets/_Game/Scripts/Boot/SaveModules/` directory

## 2. Implement EconomySaveModule

- [x] 2.1 Create `EconomySaveModule.cs` implementing `ISaveModule` with Key `"economy"`
- [x] 2.2 Implement `Serialize()` returning anonymous object `{ balance = _state.Balance.Value }`
- [x] 2.3 Implement `Deserialize(data)` casting to dynamic and setting `_state.Balance.Value`
- [x] 2.4 Implement `Validate(data, errors)` checking balance >= 0

## 3. Implement ResourceSaveModule

- [x] 3.1 Create `ResourceSaveModule.cs` implementing `ISaveModule` with Key `"resources"`
- [x] 3.2 Implement `Serialize()` returning `new Dictionary<string, int>(_state.Balances)`
- [x] 3.3 Implement `Deserialize(data)` iterating dictionary and calling `_state.SetAmount()` per entry (merge strategy)
- [x] 3.4 Implement `Validate(data, errors)` checking all values >= 0 with resource key identification

## 4. Register modules in RootLifetimeScope

- [x] 4.1 Add `builder.Register<EconomySaveModule>(Lifetime.Singleton).As<ISaveModule>()` to `Configure()`
- [x] 4.2 Add `builder.Register<ResourceSaveModule>(Lifetime.Singleton).As<ISaveModule>()` to `Configure()`
- [x] 4.3 Replace existing TODO comment about domain save modules with registration calls

## 5. Verify build and smoke test

- [x] 5.1 Confirm project compiles without errors
- [ ] 5.2 Run in Unity editor and verify no DI resolution errors on scene load
